import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {DatePipe} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {downloadCsv} from '@/core/utils/csv-export';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {
    DocStatus,
    STATUS_CLASSES,
    STATUS_LABELS,
    TeachingContext,
    Tracker,
    TrackerCell,
    TrackerRow
} from '@/teaching/models/teaching-models';
import {TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {openedFrom} from '@/teaching/utils/back-link';

const SESSION_KEY = 'teaching.sessionId';

/**
 * Who has drafted, submitted and had approved each document this term: one row
 * per teacher, subject and class; the scheme, then each week's lesson plans,
 * then record-of-work progress. Teachers see their own rows, heads of
 * department their departments, whole-school staff everything.
 */
@Component({
    selector: 'app-teaching-tracker',
    templateUrl: './tracker.component.html',
    styleUrl: '../teaching.scss',
    providers: [DatePipe]
})
export class TrackerComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/teaching/tracker'], title: 'Tracker'}
    ];
    dashboardTitle = 'Teaching Management: Tracker';

    context: TeachingContext | null = null;
    sessionId: number | null = null;
    tracker: Tracker | null = null;
    loading = false;

    departmentId: number | null = null;
    learningLevelId: number | null = null;
    subjectId: number | null = null;
    staffId: number | null = null;
    search = '';
    onlyOverdue = false;
    // Legend labels picked as a filter; none picked shows every row.
    statuses = new Set<DocStatus>();

    statusLabels = STATUS_LABELS;
    statusClasses = STATUS_CLASSES;
    legend: DocStatus[] = ['NotStarted', 'Draft', 'Submitted', 'Returned', 'Approved'];

    constructor(private svc: TeachingDocumentsService, private router: Router, private route: ActivatedRoute,
                private toastr: ToastrService, private datePipe: DatePipe) {}

    ngOnInit(): void {
        let q = this.route.snapshot.queryParamMap;
        let num = (k: string) => +(q.get(k) || 0) || null;
        this.staffId = num('teacher');
        this.departmentId = num('dept');
        this.learningLevelId = num('grade');
        this.subjectId = num('subject');
        this.search = q.get('q') || '';
        this.statuses = new Set((q.get('status') || '').split(',').filter((s) => this.legend.includes(s as DocStatus)) as DocStatus[]);
        this.onlyOverdue = q.get('overdue') === '1';
        this.svc.context().subscribe({
            next: (c) => {
                this.context = c;
                let saved: number | null = num('term');
                try { saved = saved || +(sessionStorage.getItem(SESSION_KEY) || 0) || null; } catch { /* storage blocked */ }
                this.sessionId = c.sessions.some((s) => s.id === saved) ? saved : c.currentSessionId;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading your teaching details.')
        });
    }

    // The term and filters live in the URL, so a refresh, or Back from a document
    // opened here, comes back to the same view.
    syncUrl() {
        let p: Record<string, any> = {
            term: this.sessionId, teacher: this.staffId, dept: this.departmentId, grade: this.learningLevelId,
            subject: this.subjectId, q: this.search.trim() || null,
            status: this.statuses.size ? Array.from(this.statuses).join(',') : null, overdue: this.onlyOverdue ? 1 : null
        };
        Object.keys(p).forEach((k) => { if (p[k] == null) delete p[k]; });
        this.router.navigate([], {relativeTo: this.route, queryParams: p, replaceUrl: true});
    }

    load() {
        if (!this.sessionId) return;
        this.syncUrl();
        try { sessionStorage.setItem(SESSION_KEY, String(this.sessionId)); } catch { /* storage blocked */ }
        this.loading = true;
        this.svc.tracker({sessionId: this.sessionId}).subscribe({
            next: (t) => { this.tracker = t; this.loading = false; },
            error: (err) => { this.loading = false; this.toastr.error(err.error?.message || 'Error loading the tracker.'); }
        });
    }

    // Filter options come from the rows themselves; the teacher comes first and narrows the rest.
    get teachers() { return this.distinct((r) => r.staffDetailsId, (r) => r.staffName); }
    get departments() { return this.distinct((r) => r.departmentId, (r) => r.departmentName || '-', undefined, (r) => this.ofTeacher(r)); }
    // Grades in their set order (Learning Levels > Rank).
    get grades() {
        return this.distinct((r) => r.learningLevelId, (r) => r.learningLevelName, (r) => r.learningLevelRank, (r) => this.ofTeacher(r));
    }
    // Subjects in their set order (Subjects > Rank), narrowed to the chosen department and grade.
    get subjects() {
        return this.distinct((r) => r.subjectId, (r) => r.subjectName, (r) => r.subjectRank,
            (r) => this.ofTeacher(r) && (this.departmentId == null || r.departmentId === this.departmentId)
                && (this.learningLevelId == null || r.learningLevelId === this.learningLevelId));
    }

    private ofTeacher(r: TrackerRow): boolean {
        return this.staffId == null || r.staffDetailsId === this.staffId;
    }

    // A teacher change drops a department or grade they don't teach in, then narrows the subject.
    narrowToTeacher() {
        if (this.departmentId != null && !this.departments.some((d) => d.id === this.departmentId)) this.departmentId = null;
        if (this.learningLevelId != null && !this.grades.some((g) => g.id === this.learningLevelId)) this.learningLevelId = null;
        this.narrowSubject();
    }

    // A grade or department change drops a subject not taught there.
    narrowSubject() {
        if (this.subjectId != null && !this.subjects.some((s) => s.id === this.subjectId)) this.subjectId = null;
        this.syncUrl();
    }

    toggleStatus(s: DocStatus) {
        if (!this.statuses.delete(s)) this.statuses.add(s);
        this.syncUrl();
    }

    toggleOverdue() {
        this.onlyOverdue = !this.onlyOverdue;
        this.syncUrl();
    }

    clearStatus() {
        this.statuses.clear();
        this.onlyOverdue = false;
        this.syncUrl();
    }

    chipClass(on: boolean): string {
        return on ? 'td-filter-on' : (this.statuses.size || this.onlyOverdue) ? 'td-filter-off' : '';
    }

    // Returned covers rejected documents too, as the legend has no label of its own for them.
    private hasStatus(r: TrackerRow): boolean {
        return [r.scheme, ...r.weeks].some((c) =>
            this.statuses.has(c.status) || (c.status === 'Rejected' && this.statuses.has('Returned')));
    }

    // Sorted by rank when one is given, else by name.
    private distinct(id: (r: TrackerRow) => number, name: (r: TrackerRow) => string,
                     rank?: (r: TrackerRow) => number, where: (r: TrackerRow) => boolean = () => true): {id: number; name: string}[] {
        let map = new Map<number, {id: number; name: string; rank: number}>();
        (this.tracker?.rows || []).filter(where).forEach((r) => {
            if (!map.has(id(r))) map.set(id(r), {id: id(r), name: name(r), rank: rank ? rank(r) : 0});
        });
        return Array.from(map.values())
            .sort((a, b) => a.rank - b.rank || a.name.localeCompare(b.name, undefined, {numeric: true}))
            .map(({id, name}) => ({id, name}));
    }

    get rows(): TrackerRow[] {
        let q = this.search.trim().toLowerCase();
        return (this.tracker?.rows || []).filter((r) =>
            (this.departmentId == null || r.departmentId === this.departmentId)
            && (this.learningLevelId == null || r.learningLevelId === this.learningLevelId)
            && (this.subjectId == null || r.subjectId === this.subjectId)
            && (this.staffId == null || r.staffDetailsId === this.staffId)
            && (!this.statuses.size || this.hasStatus(r))
            && (!q || [r.staffName, r.subjectName, r.className].some((v) => v.toLowerCase().includes(q)))
            && (!this.onlyOverdue || r.scheme.overdue || r.weeks.some((w) => w.overdue)));
    }

    // Per column: how many are in (submitted or approved) out of those due.
    columnCount(cells: TrackerCell[]): string {
        let applicable = cells.filter((c) => c.status !== 'NotApplicable');
        let done = applicable.filter((c) => c.status === 'Submitted' || c.status === 'Approved').length;
        return applicable.length ? `${done}/${applicable.length}` : '';
    }

    get schemeCount(): string { return this.columnCount(this.rows.map((r) => r.scheme)); }
    weekCount(i: number): string { return this.columnCount(this.rows.map((r) => r.weeks[i])); }

    cellTitle(c: TrackerCell, what: string, due?: string): string {
        let s = `${what}: ${this.statusLabels[c.status]}`;
        if (c.awaitingName) s += ` - awaiting ${c.awaitingName}`;
        if (due && c.status !== 'NotApplicable') s += ` - due ${this.datePipe.transform(due, 'EEE d MMM HH:mm')}`;
        if (c.overdue) s += ' - OVERDUE';
        return s;
    }

    short(c: TrackerCell): string {
        switch (c.status) {
            case 'Draft': return 'D';
            case 'Submitted': return 'S';
            case 'Returned': return 'R';
            case 'Rejected': return 'X';
            case 'Approved': return '✓';
            default: return '';
        }
    }

    openScheme(r: TrackerRow) {
        if (r.scheme.id) this.router.navigate(['/teaching/schemes'], {queryParams: openedFrom(this.router, {schemeId: r.scheme.id})});
    }

    openWeek(r: TrackerRow, c: TrackerCell) {
        if (c.id) this.router.navigate(['/teaching/lesson-plans'], {queryParams: openedFrom(this.router, {weekId: c.id})});
        else if (r.scheme.id && c.status !== 'NotApplicable') this.router.navigate(['/teaching/lesson-plans'], {queryParams: openedFrom(this.router, {schemeId: r.scheme.id})});
    }

    openRecord(r: TrackerRow) {
        if (r.scheme.id) this.router.navigate(['/teaching/record-of-work'], {queryParams: openedFrom(this.router, {schemeId: r.scheme.id})});
    }

    percent(r: TrackerRow): number {
        return r.plannedLessons ? Math.round(100 * r.taughtLessons / r.plannedLessons) : 0;
    }

    exportCsv() {
        let t = this.tracker!;
        downloadCsv(`Teaching management tracker ${t.session.name} ${t.session.academicYearName}`, [
            {header: 'Teacher', value: (r: TrackerRow) => r.staffName},
            {header: 'Department', value: (r: TrackerRow) => r.departmentName},
            {header: 'Subject', value: (r: TrackerRow) => r.subjectName},
            {header: 'Class', value: (r: TrackerRow) => r.className},
            {header: 'Scheme', value: (r: TrackerRow) => this.statusLabels[r.scheme.status] + (r.scheme.overdue ? ' (overdue)' : '')},
            ...t.weeks.map((w, i) => ({
                header: `Week ${w.weekNo}`,
                value: (r: TrackerRow) => r.weeks[i].status === 'NotApplicable' ? '' : this.statusLabels[r.weeks[i].status] + (r.weeks[i].overdue ? ' (overdue)' : '')
            })),
            {header: 'Lessons taught', value: (r: TrackerRow) => r.taughtLessons},
            {header: 'Lessons planned', value: (r: TrackerRow) => r.plannedLessons},
            {header: 'Weeks checked', value: (r: TrackerRow) => r.checkedWeeks}
        ], this.rows);
    }

    print() {
        let t = this.tracker!;
        let colours: Record<string, string> = {NotStarted: '#e2e3e5', Draft: '#ffe08a', Submitted: '#9ec5fe', Returned: '#f1aeb5',
            Rejected: '#f1aeb5', Approved: '#75b798', NotApplicable: '#fff'};
        let cell = (c: TrackerCell) => `<td class="c" style="background:${colours[c.status]};${c.overdue ? 'outline:2px solid #dc3545;outline-offset:-2px;' : ''}">${this.short(c)}</td>`;
        let body = `<h1>Teaching management - ${escapeHtml(t.session.name)} ${escapeHtml(t.session.academicYearName)}</h1>
            <p class="legend">${this.legend.map((s) => `<span style="background:${colours[s]}">${this.statusLabels[s]}</span>`).join(' ')}
                <span style="outline:2px solid #dc3545">Overdue</span> &nbsp; D draft, S submitted, R returned, ✓ approved</p>
            <table><thead><tr><th>Teacher</th><th>Subject</th><th>Class</th><th>Scheme</th>
                ${t.weeks.map((w) => `<th class="c">W${w.weekNo}</th>`).join('')}<th>Taught</th></tr></thead>
            <tbody>${this.rows.map((r) => `<tr><td>${escapeHtml(r.staffName)}</td><td>${escapeHtml(r.subjectName)}</td>
                <td>${escapeHtml(r.className)}</td>${cell(r.scheme)}${r.weeks.map(cell).join('')}
                <td class="c">${r.taughtLessons}/${r.plannedLessons}</td></tr>`).join('')}</tbody></table>`;
        let css = `@page { size: A4 landscape; margin: 8mm; }
            body { font-family: Arial, Helvetica, sans-serif; font-size: 9px; color: #111; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
            h1 { font-size: 13px; margin: 0 0 4px; } .legend span { padding: 1px 6px; margin-right: 4px; }
            table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #999; padding: 2px 3px; text-align: left; }
            .c { text-align: center; } tr { page-break-inside: avoid; }`;
        printInFrame(`Teaching management tracker ${t.session.name} ${t.session.academicYearName}`, css, body, 'landscape');
    }
}
