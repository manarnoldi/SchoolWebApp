import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {DatePipe} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {downloadCsv} from '@/core/utils/csv-export';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {TeachingContext} from '@/teaching/models/teaching-models';
import {TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {AssessmentTypeService} from '@/cbe/assessments/services/assessment-type.service';
import {
    AssessmentTracker,
    AssessmentTrackerDetail,
    AssessmentTrackerRow,
    AssessmentTrackerService,
    AssessmentTrackStatus,
    TRACK_STATUS_CLASSES,
    TRACK_STATUS_HELP,
    TRACK_STATUS_LABELS
} from '@/cbe/assessments/services/assessment-tracker.service';

const SESSION_KEY = 'assessmentTracker.sessionId';

interface TeacherTotal {
    staffDetailsId: number;
    staffName: string;
    rows: number;
    upToDate: number;
    incomplete: number;
    behind: number;
    noScheme: number;
    outcomesDue: number;
    outcomesGraded: number;
    outcomesPartlyGraded: number;
    outcomesOverdue: number;
    lastEntry: string | null;
}

/**
 * How far each teacher has got with formative assessments this term: one row per
 * teacher, learning area and class. The scheme of work says which sub-strands'
 * outcomes are due (a week after the last week teaching them); the grades entered
 * say which are done. A row opens to its sub-strands and the learners missing
 * grades, each sub-strand linking to Student Assessments. Teachers see their own
 * rows, heads of department their departments, admins the whole school.
 */
@Component({
    selector: 'app-assessment-tracker',
    templateUrl: './assessment-tracker.component.html',
    styles: [`
        .at-scroll { max-height: 72vh; overflow: auto; }
        .at-scroll thead th { position: sticky; top: 0; z-index: 2; background: #f4f6f9; }
        .at-bar { display: flex; gap: 2px; height: 10px; border-radius: 4px; overflow: hidden; background: #e9ecef; min-width: 90px; }
        .at-bar > div { min-width: 2px; }
        .at-row { cursor: pointer; }
        .at-row.open > td { background: #eef4fb; }
        .at-detail > td { background: #fafbfc; }
        .at-chip { display: inline-block; min-width: 2.6em; padding: 0 4px; margin: 1px; border-radius: 4px; font-size: .7rem; text-align: center; }
        .at-missing { max-height: 320px; overflow-y: auto; }
        .at-filter { cursor: pointer; user-select: none; }
        .at-filter.off { opacity: .4; }
        .at-filter.on { outline: 2px solid #343a40; outline-offset: 1px; }
        .badge { font-weight: 500; }
    `],
    providers: [DatePipe]
})
export class AssessmentTrackerComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/assessments/tracker'], title: 'Assessment Tracker'}
    ];
    dashboardTitle = 'Assessments: Tracker';

    context: TeachingContext | null = null;
    sessionId: number | null = null;
    tracker: AssessmentTracker | null = null;
    loading = false;

    view: 'classes' | 'teachers' = 'classes';
    departmentId: number | null = null;
    learningLevelId: number | null = null;
    subjectId: number | null = null;
    staffId: number | null = null;
    search = '';
    statuses = new Set<AssessmentTrackStatus>();

    legend: AssessmentTrackStatus[] = ['UpToDate', 'Incomplete', 'Behind', 'NotStarted', 'NothingDue', 'NoScheme'];
    labels = TRACK_STATUS_LABELS;
    classes = TRACK_STATUS_CLASSES;
    help = TRACK_STATUS_HELP;

    // The row opened, and its sub-strands and learners missing grades.
    openKey: string | null = null;
    detail: AssessmentTrackerDetail | null = null;
    detailLoading = false;
    // Formative assessments, the type Student Assessments opens on from here.
    assessmentTypeId: number | null = null;

    constructor(private svc: AssessmentTrackerService, private teachingSvc: TeachingDocumentsService,
                private assessmentTypeSvc: AssessmentTypeService, private router: Router, private route: ActivatedRoute,
                private toastr: ToastrService, private datePipe: DatePipe) {}

    ngOnInit(): void {
        let q = this.route.snapshot.queryParamMap;
        let num = (k: string) => +(q.get(k) || 0) || null;
        this.staffId = num('teacher');
        this.departmentId = num('dept');
        this.learningLevelId = num('grade');
        this.subjectId = num('subject');
        this.search = q.get('q') || '';
        this.view = q.get('view') === 'teachers' ? 'teachers' : 'classes';
        this.statuses = new Set((q.get('status') || '').split(',').filter((s) => this.legend.includes(s as AssessmentTrackStatus)) as AssessmentTrackStatus[]);
        this.assessmentTypeSvc.get('/assessmentTypes').subscribe({
            next: (types: any[]) => {
                let formative = types.find((t) => /formative/i.test(t.name)) || types[0];
                this.assessmentTypeId = formative?.id ?? null;
            },
            error: () => {}
        });
        this.teachingSvc.context().subscribe({
            next: (c) => {
                this.context = c;
                let saved: number | null = num('term');
                try { saved = saved || +(sessionStorage.getItem(SESSION_KEY) || 0) || null; } catch { /* storage blocked */ }
                this.sessionId = c.sessions.some((s) => s.id === saved) ? saved : c.currentSessionId;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the terms.')
        });
    }

    // The term, view and filters live in the URL, so a refresh, or Back from
    // Student Assessments, comes back to the same view.
    syncUrl() {
        let p: Record<string, any> = {
            term: this.sessionId, view: this.view === 'teachers' ? 'teachers' : null, teacher: this.staffId,
            dept: this.departmentId, grade: this.learningLevelId, subject: this.subjectId, q: this.search.trim() || null,
            status: this.statuses.size ? Array.from(this.statuses).join(',') : null
        };
        Object.keys(p).forEach((k) => { if (p[k] == null) delete p[k]; });
        this.router.navigate([], {relativeTo: this.route, queryParams: p, replaceUrl: true});
    }

    load() {
        if (!this.sessionId) return;
        this.syncUrl();
        try { sessionStorage.setItem(SESSION_KEY, String(this.sessionId)); } catch { /* storage blocked */ }
        this.loading = true;
        this.closeDetail();
        this.svc.tracker(this.sessionId).subscribe({
            next: (t) => { this.tracker = t; this.loading = false; },
            error: (err) => { this.loading = false; this.toastr.error(err.error?.message || 'Error loading the tracker.'); }
        });
    }

    setView(v: 'classes' | 'teachers') {
        this.view = v;
        this.syncUrl();
    }

    // ---------- Filters (options come from the rows themselves) ----------

    get teachers() { return this.distinct((r) => r.staffDetailsId, (r) => r.staffName); }
    get departments() { return this.distinct((r) => r.departmentId, (r) => r.departmentName || '-', undefined, (r) => this.ofTeacher(r)); }
    get grades() {
        return this.distinct((r) => r.learningLevelId, (r) => r.learningLevelName, (r) => r.learningLevelRank, (r) => this.ofTeacher(r));
    }
    get subjects() {
        return this.distinct((r) => r.subjectId, (r) => r.subjectName, (r) => r.subjectRank,
            (r) => this.ofTeacher(r) && (this.departmentId == null || r.departmentId === this.departmentId)
                && (this.learningLevelId == null || r.learningLevelId === this.learningLevelId));
    }

    private ofTeacher(r: AssessmentTrackerRow): boolean {
        return this.staffId == null || r.staffDetailsId === this.staffId;
    }

    narrowToTeacher() {
        if (this.departmentId != null && !this.departments.some((d) => d.id === this.departmentId)) this.departmentId = null;
        if (this.learningLevelId != null && !this.grades.some((g) => g.id === this.learningLevelId)) this.learningLevelId = null;
        this.narrowSubject();
    }

    narrowSubject() {
        if (this.subjectId != null && !this.subjects.some((s) => s.id === this.subjectId)) this.subjectId = null;
        this.syncUrl();
    }

    toggleStatus(s: AssessmentTrackStatus) {
        if (!this.statuses.delete(s)) this.statuses.add(s);
        this.syncUrl();
    }

    clearStatus() {
        this.statuses.clear();
        this.syncUrl();
    }

    chipState(s: AssessmentTrackStatus): string {
        return this.statuses.has(s) ? 'on' : this.statuses.size ? 'off' : '';
    }

    private distinct(id: (r: AssessmentTrackerRow) => number, name: (r: AssessmentTrackerRow) => string,
                     rank?: (r: AssessmentTrackerRow) => number, where: (r: AssessmentTrackerRow) => boolean = () => true): {id: number; name: string}[] {
        let map = new Map<number, {id: number; name: string; rank: number}>();
        (this.tracker?.rows || []).filter(where).forEach((r) => {
            if (!map.has(id(r))) map.set(id(r), {id: id(r), name: name(r), rank: rank ? rank(r) : 0});
        });
        return Array.from(map.values())
            .sort((a, b) => a.rank - b.rank || a.name.localeCompare(b.name, undefined, {numeric: true}))
            .map(({id, name}) => ({id, name}));
    }

    // Rows matching every filter but the status - what the legend counts.
    private get filtered(): AssessmentTrackerRow[] {
        let q = this.search.trim().toLowerCase();
        return (this.tracker?.rows || []).filter((r) =>
            (this.departmentId == null || r.departmentId === this.departmentId)
            && (this.learningLevelId == null || r.learningLevelId === this.learningLevelId)
            && (this.subjectId == null || r.subjectId === this.subjectId)
            && (this.staffId == null || r.staffDetailsId === this.staffId)
            && (!q || [r.staffName, r.subjectName, r.className].some((v) => v.toLowerCase().includes(q))));
    }

    get rows(): AssessmentTrackerRow[] {
        return this.filtered.filter((r) => !this.statuses.size || this.statuses.has(r.status));
    }

    count(s: AssessmentTrackStatus): number {
        return this.filtered.filter((r) => r.status === s).length;
    }

    // One line per teacher: their subject-classes by status, and outcomes graded against due.
    get teacherTotals(): TeacherTotal[] {
        let map = new Map<number, TeacherTotal>();
        this.rows.forEach((r) => {
            let t = map.get(r.staffDetailsId);
            if (!t) {
                t = {staffDetailsId: r.staffDetailsId, staffName: r.staffName, rows: 0, upToDate: 0, incomplete: 0, behind: 0,
                    noScheme: 0, outcomesDue: 0, outcomesGraded: 0, outcomesPartlyGraded: 0, outcomesOverdue: 0, lastEntry: null};
                map.set(r.staffDetailsId, t);
            }
            t.rows++;
            if (r.status === 'UpToDate') t.upToDate++;
            if (r.status === 'Incomplete') t.incomplete++;
            if (r.status === 'Behind' || r.status === 'NotStarted') t.behind++;
            if (r.status === 'NoScheme') t.noScheme++;
            t.outcomesDue += r.outcomesDue;
            t.outcomesGraded += r.outcomesGraded;
            t.outcomesPartlyGraded += r.outcomesPartlyGraded;
            t.outcomesOverdue += r.outcomesOverdue;
            if (r.lastEntry && (!t.lastEntry || r.lastEntry > t.lastEntry)) t.lastEntry = r.lastEntry;
        });
        // Furthest behind first.
        return Array.from(map.values()).sort((a, b) => b.outcomesOverdue - a.outcomesOverdue || b.behind - a.behind
            || a.staffName.localeCompare(b.staffName));
    }

    showTeacher(t: TeacherTotal) {
        this.staffId = t.staffDetailsId;
        this.view = 'classes';
        this.narrowToTeacher();
    }

    // ---------- Figures ----------

    pct(n: number, of: number): number {
        return of ? Math.round((100 * n) / of) : 0;
    }

    // Outcomes due: graded, partly graded, not graded.
    bar(r: {outcomesGraded: number; outcomesPartlyGraded: number; outcomesOverdue: number}) {
        return [
            {n: r.outcomesGraded, fill: '#1f7a4d', label: 'graded'},
            {n: r.outcomesPartlyGraded, fill: '#e8913a', label: 'partly graded'},
            {n: r.outcomesOverdue, fill: '#b02a37', label: 'not graded'}
        ].filter((s) => s.n > 0);
    }

    rowTitle(r: AssessmentTrackerRow): string {
        if (r.status === 'NoScheme') return `No scheme of work - ${r.outcomesGradedThisTerm} outcome(s) graded this term`;
        let s = `${r.outcomesDue} outcome(s) due from ${r.subStrandsDue} of ${r.subStrandsPlanned} sub-strand(s): `
            + `${r.outcomesGraded} graded, ${r.outcomesPartlyGraded} partly, ${r.outcomesOverdue} not graded`;
        if (r.nextSubStrand) s += `. Next: ${r.nextSubStrand}, due ${this.datePipe.transform(r.nextDueAt, 'EEE d MMM', 'UTC')}`;
        return s;
    }

    // ---------- Detail ----------

    key(r: AssessmentTrackerRow): string {
        return `${r.staffDetailsId}-${r.subjectId}-${r.schoolClassId}`;
    }

    toggle(r: AssessmentTrackerRow) {
        if (this.openKey === this.key(r)) {
            this.closeDetail();
            return;
        }
        this.openKey = this.key(r);
        this.detail = null;
        this.detailLoading = true;
        this.svc.detail(this.sessionId!, r).subscribe({
            next: (d) => {
                if (this.openKey !== this.key(r)) return;
                this.detail = d;
                this.detailLoading = false;
            },
            error: (err) => {
                this.detailLoading = false;
                this.toastr.error(err.error?.message || 'Error loading the details.');
            }
        });
    }

    closeDetail() {
        this.openKey = null;
        this.detail = null;
    }

    chipClass(graded: number, learners: number): string {
        if (!graded) return 'bg-danger-subtle text-danger-emphasis';
        return learners && graded < Math.ceil(learners * 0.8) ? 'bg-warning-subtle text-warning-emphasis' : 'bg-success-subtle text-success-emphasis';
    }

    // Student Assessments on this row's class and learning area, as its teacher; on a
    // sub-strand, with its grade sheet loaded.
    openAssessments(r: AssessmentTrackerRow, s?: {strandId: number; subStrandId: number}) {
        let t = this.tracker!;
        let p: Record<string, any> = {
            curriculumId: t.curriculumId,
            academicYearId: t.session.academicYearId,
            sessionId: t.session.id,
            schoolClassId: r.schoolClassId,
            subjectId: r.subjectId,
            staffDetailsId: r.staffDetailsId,
            assessmentTypeId: this.assessmentTypeId,
            from: this.router.url
        };
        if (s) Object.assign(p, {strandId: s.strandId, subStrandId: s.subStrandId, loaded: 1});
        Object.keys(p).forEach((k) => { if (p[k] == null) delete p[k]; });
        this.router.navigate(['/cbe/assessments/assessments'], {queryParams: p});
    }

    // ---------- Export ----------

    private get title(): string {
        let t = this.tracker!;
        return `Assessment tracker ${t.session.name} ${t.session.academicYearName}`;
    }

    private date(d: string | null): string {
        return d ? this.datePipe.transform(d, 'd MMM yyyy', 'UTC') || '' : '';
    }

    exportCsv() {
        if (this.view === 'teachers') {
            downloadCsv(this.title + ' by teacher', [
                {header: 'Teacher', value: (t: TeacherTotal) => t.staffName},
                {header: 'Subject-classes', value: (t: TeacherTotal) => t.rows},
                {header: 'Up to date', value: (t: TeacherTotal) => t.upToDate},
                {header: 'Incomplete', value: (t: TeacherTotal) => t.incomplete},
                {header: 'Behind or not started', value: (t: TeacherTotal) => t.behind},
                {header: 'No scheme', value: (t: TeacherTotal) => t.noScheme},
                {header: 'Outcomes due', value: (t: TeacherTotal) => t.outcomesDue},
                {header: 'Outcomes graded', value: (t: TeacherTotal) => t.outcomesGraded},
                {header: 'Outcomes not graded', value: (t: TeacherTotal) => t.outcomesOverdue},
                {header: 'Last entry', value: (t: TeacherTotal) => this.date(t.lastEntry)}
            ], this.teacherTotals);
            return;
        }
        downloadCsv(this.title, [
            {header: 'Teacher', value: (r: AssessmentTrackerRow) => r.staffName},
            {header: 'Department', value: (r: AssessmentTrackerRow) => r.departmentName},
            {header: 'Learning area', value: (r: AssessmentTrackerRow) => r.subjectName},
            {header: 'Class', value: (r: AssessmentTrackerRow) => r.className},
            {header: 'Learners', value: (r: AssessmentTrackerRow) => r.learners},
            {header: 'Sub-strands due', value: (r: AssessmentTrackerRow) => r.subStrandsDue},
            {header: 'Sub-strands in scheme', value: (r: AssessmentTrackerRow) => r.subStrandsPlanned},
            {header: 'Outcomes due', value: (r: AssessmentTrackerRow) => r.outcomesDue},
            {header: 'Outcomes graded', value: (r: AssessmentTrackerRow) => r.outcomesGraded},
            {header: 'Outcomes partly graded', value: (r: AssessmentTrackerRow) => r.outcomesPartlyGraded},
            {header: 'Outcomes not graded', value: (r: AssessmentTrackerRow) => r.outcomesOverdue},
            {header: 'Grades entered %', value: (r: AssessmentTrackerRow) => r.gradesExpected ? this.pct(r.gradesEntered, r.gradesExpected) : ''},
            {header: 'Last entry', value: (r: AssessmentTrackerRow) => this.date(r.lastEntry)},
            {header: 'Status', value: (r: AssessmentTrackerRow) => this.labels[r.status]}
        ], this.rows);
    }

    print() {
        let colours: Record<AssessmentTrackStatus, string> = {UpToDate: '#d1e7dd', Incomplete: '#fff3cd', Behind: '#f8d7da',
            NotStarted: '#f1aeb5', NothingDue: '#f8f9fa', NoScheme: '#e2e3e5'};
        let body: string;
        if (this.view === 'teachers') {
            body = `<table><thead><tr><th>Teacher</th><th class="c">Subject-classes</th><th class="c">Up to date</th><th class="c">Incomplete</th>
                <th class="c">Behind</th><th class="c">No scheme</th><th class="c">Outcomes graded / due</th><th class="c">Not graded</th><th>Last entry</th></tr></thead>
                <tbody>${this.teacherTotals.map((t) => `<tr><td>${escapeHtml(t.staffName)}</td><td class="c">${t.rows}</td><td class="c">${t.upToDate}</td>
                <td class="c">${t.incomplete}</td><td class="c">${t.behind}</td><td class="c">${t.noScheme}</td>
                <td class="c">${t.outcomesGraded} / ${t.outcomesDue}</td><td class="c">${t.outcomesOverdue}</td><td>${this.date(t.lastEntry)}</td></tr>`).join('')}</tbody></table>`;
        } else {
            body = `<table><thead><tr><th>Teacher</th><th>Learning area</th><th>Class</th><th class="c">Learners</th><th class="c">Sub-strands due</th>
                <th class="c">Outcomes graded / due</th><th class="c">Not graded</th><th class="c">Grades entered</th><th>Last entry</th><th>Status</th></tr></thead>
                <tbody>${this.rows.map((r) => `<tr><td>${escapeHtml(r.staffName)}</td><td>${escapeHtml(r.subjectName)}</td><td>${escapeHtml(r.className)}</td>
                <td class="c">${r.learners}</td><td class="c">${r.schemeOfWorkId ? r.subStrandsDue + ' / ' + r.subStrandsPlanned : '-'}</td>
                <td class="c">${r.schemeOfWorkId ? r.outcomesGraded + ' / ' + r.outcomesDue : r.outcomesGradedThisTerm + ' graded'}</td>
                <td class="c">${r.outcomesOverdue || ''}</td><td class="c">${r.gradesExpected ? this.pct(r.gradesEntered, r.gradesExpected) + '%' : ''}</td>
                <td>${this.date(r.lastEntry)}</td><td style="background:${colours[r.status]}">${this.labels[r.status]}</td></tr>`).join('')}</tbody></table>`;
        }
        let html = `<h1>${escapeHtml(this.title)}</h1>
            <p class="note">Printed ${this.datePipe.transform(new Date(), 'd MMM yyyy HH:mm')}. A sub-strand's outcomes are due ${this.tracker!.graceDays} days
                after the last scheme week that teaches it, or by the end of term.</p>${body}`;
        let css = `@page { size: A4 landscape; margin: 8mm; }
            body { font-family: Arial, Helvetica, sans-serif; font-size: 9px; color: #111; -webkit-print-color-adjust: exact; print-color-adjust: exact; }
            h1 { font-size: 13px; margin: 0 0 2px; } .note { margin: 0 0 6px; color: #555; }
            table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #999; padding: 2px 4px; text-align: left; }
            th { background: #eee; } .c { text-align: center; } tr { page-break-inside: avoid; }`;
        printInFrame(this.title, css, html, 'landscape');
    }
}
