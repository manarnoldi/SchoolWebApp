import {AfterViewInit, Component, ElementRef, Input, NgZone, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {forkJoin} from 'rxjs';
import {ProfileExam, ProfilesService} from '@/core/services/profiles.service';
import {AccessService} from '@/core/services/access.service';
import {escapeHtml, printInFrame, removePrintFrame} from '@/shared/utils/print-frame';

interface ChartPoint {
    x: number;
    y: number;
    v: number;
    label: string;
    color: string;
    grade?: string | null;
}

interface Series {
    name: string;
    color: string;
    dashed: boolean;
    points: ChartPoint[];
    on: boolean;
}

// Fixed categorical order (dataviz palette); a subject keeps its colour by its set order.
const SLOTS = ['#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4', '#008300', '#6250d6', '#e34948'];
const OTHER = '#8f8d87';
// Against the class mean: at or above, below.
const ABOVE = '#198754';
const BELOW = '#dc3545';
const NEUTRAL = '#2a78d6';

const PRINT_CSS = `
    body { font-family: Arial, Helvetica, sans-serif; color: #212529; margin: 12mm; }
    h1 { font-size: 18px; margin: 0 0 2px; } .sub { color: #52514e; font-size: 12px; margin-bottom: 10px; }
    svg { width: 100%; height: auto; } svg text { font-size: 11px; font-family: Arial, Helvetica, sans-serif; }
    svg text:not(.val):not(.grade) { fill: #52514e; } svg .val { font-weight: bold; }
    svg text.grade { font-size: 9.5px; font-weight: bold; fill: #6c757d; }
    table { width: 100%; border-collapse: collapse; font-size: 12px; margin-top: 10px; }
    th, td { border: 1px solid #dee2e6; padding: 4px 6px; text-align: left; } th { background: #e7f1ff; }
    td.n { text-align: right; } .up { color: ${ABOVE}; font-weight: bold; } .down { color: ${BELOW}; font-weight: bold; }
`;

/**
 * Student page, Academics tab: for a year (the current one first), a progress
 * chart across the whole year - the student's mean (green at or above the class
 * mean, red below) against the class mean, or subject by subject - then each
 * exam's marks, filtered by term; and prints: the report forms, the assessment
 * report and this progress report.
 */
@Component({
    selector: 'app-student-academics',
    styles: [`
        :host { display: block; }
        .exam-head { cursor: pointer; background: #e7f1ff; color: #084298; }
        .exam-head:hover { background: #dbe9ff; }
        .exam.open > .exam-head { background: #0d6efd; color: #fff; }
        .exam.open > .exam-head .text-muted { color: #e7f1ff !important; }
        .bar { height: 8px; background: #e9ecef; border-radius: 4px; overflow: hidden; min-width: 80px; }
        .bar > div { height: 100%; background: #2a78d6; }
        .chart { display: block; }
        .chart text:not(.val):not(.grade) { font-size: 11px; fill: #52514e; }
        .chart .val { font-size: 10.5px; font-weight: 600; }
        .chart text.grade { font-size: 9.5px; font-weight: 600; fill: #6c757d; }
        .key { display: inline-flex; align-items: center; gap: 6px; margin: 2px 14px 2px 0; font-size: .8rem; color: #212529; }
        .key.toggle { cursor: pointer; user-select: none; }
        .key.off { opacity: .35; }
        .sw { width: 12px; height: 12px; border-radius: 3px; display: inline-block; }
        .dot { width: 9px; height: 9px; border-radius: 50%; display: inline-block; }
        /* Grade badges, in the rubric colours: exceeding, meeting, approaching, below expectations */
        .gr { display: inline-block; min-width: 2.1rem; padding: 0 5px; border-radius: 4px; font-size: .72rem; font-weight: 700;
            text-align: center; line-height: 1.5; background: #e9ecef; color: #495057; }
        .gr-ee { background: #d1e7dd; color: #0f5132; } .gr-me { background: #cfe2ff; color: #084298; }
        .gr-ae { background: #fff3cd; color: #664d03; } .gr-be { background: #f8d7da; color: #842029; }
        .exam.open > .exam-head .gr { box-shadow: 0 0 0 1px #fff; }
    `],
    template: `
        <div class="d-flex flex-wrap align-items-end gap-2 mb-2">
            <div>
                <label class="form-label mb-0 small">Year</label>
                <select class="form-select form-select-sm" [(ngModel)]="yearId" (ngModelChange)="sessionId = null; refresh()">
                    <option *ngFor="let y of years" [ngValue]="y.id">{{ y.name }}</option>
                </select>
            </div>
            <div>
                <label class="form-label mb-0 small">Term <span class="text-muted">(the list below)</span></label>
                <select class="form-select form-select-sm" [(ngModel)]="sessionId" (ngModelChange)="refreshList()">
                    <option [ngValue]="null">All terms</option>
                    <option *ngFor="let s of sessions" [ngValue]="s.id">{{ s.name }}</option>
                </select>
            </div>
            <div class="dropdown ms-auto" *ngIf="yearExams.length">
                <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="fas fa-print"></i> Print</button>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li *ngIf="can.reportForms && printTerm"><a class="dropdown-item" [routerLink]="['/reports/academics/exam-results']"
                        [queryParams]="link('reportForm', true)"><i class="fas fa-file-alt text-primary me-2"></i>Report form - {{ printTerm.session }}</a></li>
                    <li *ngIf="can.reportForms"><a class="dropdown-item" [routerLink]="['/reports/academics/exam-results']"
                        [queryParams]="link('annualReportForm', false)"><i class="fas fa-calendar-check text-primary me-2"></i>Annual report form - {{ yearName }}</a></li>
                    <li *ngIf="can.assessment && printTerm"><a class="dropdown-item" [routerLink]="['/reports/academics/assessment-report']"
                        [queryParams]="link(null, true)"><i class="fas fa-clipboard-check text-primary me-2"></i>Assessment report - {{ printTerm.session }}</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li><a class="dropdown-item" href="javascript:void(0)" (click)="printProgress()">
                        <i class="fas fa-chart-line text-primary me-2"></i>Progress report - {{ yearName }}</a></li>
                </ul>
            </div>
        </div>

        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading the exam results...</div>
        <div *ngIf="!loading && yearExams.length === 0" class="text-muted p-2">No exam results for this year.</div>

        <!-- Progress across the year -->
        <div class="card mb-2" *ngIf="!loading && yearExams.length > 1">
            <div class="card-header py-1 d-flex flex-wrap align-items-center gap-2">
                <b class="me-auto"><i class="fas fa-chart-line text-primary me-1"></i>Progress through {{ yearName }}</b>
                <div class="btn-group btn-group-sm">
                    <button class="btn" [ngClass]="view === 'overall' ? 'btn-primary' : 'btn-outline-primary'" (click)="setView('overall')">Overall</button>
                    <button class="btn" [ngClass]="view === 'subjects' ? 'btn-primary' : 'btn-outline-primary'" (click)="setView('subjects')">By subject</button>
                </div>
            </div>
            <div class="card-body p-2">
                <div class="mb-1" *ngIf="view === 'overall'">
                    <span class="key"><span class="dot" [style.background]="above"></span>At or above the class mean</span>
                    <span class="key"><span class="dot" [style.background]="below"></span>Below the class mean</span>
                    <span class="key" *ngIf="hasClassMeans"><svg width="22" height="8"><line x1="0" y1="4" x2="22" y2="4" stroke="#6c757d" stroke-width="2" stroke-dasharray="5 3"/></svg>Class mean</span>
                </div>
                <div class="mb-1" *ngIf="view === 'subjects'">
                    <span *ngFor="let s of series" class="key toggle" [class.off]="!s.on" (click)="s.on = !s.on" [title]="'Show or hide ' + s.name">
                        <span class="sw" [style.background]="s.color"></span>{{ s.name }}</span>
                    <small class="text-muted">- click a subject to show or hide it</small>
                </div>
                <!-- Drawn at the panel's own width; a long year scrolls sideways -->
                <div style="overflow-x: auto;">
                    <svg class="chart" #chartSvg xmlns="http://www.w3.org/2000/svg" [attr.width]="W" [attr.height]="H"
                        [attr.viewBox]="'0 0 ' + W + ' ' + H" role="img" [attr.aria-label]="'Marks through ' + yearName">
                        <g *ngFor="let g of grid">
                            <line [attr.x1]="padL" [attr.x2]="W - padR" [attr.y1]="yFor(g)" [attr.y2]="yFor(g)" stroke="#e9ecef"/>
                            <text [attr.x]="padL - 6" [attr.y]="yFor(g) + 4" text-anchor="end">{{ g }}%</text>
                        </g>
                        <g *ngFor="let t of termStarts">
                            <line [attr.x1]="t.x" [attr.x2]="t.x" [attr.y1]="padT - 8" [attr.y2]="H - padB" stroke="#cfe2ff" stroke-dasharray="3 3"/>
                            <text [attr.x]="t.x + 4" [attr.y]="padT - 10" style="font-weight: 600; fill: #084298;">{{ t.name }}</text>
                        </g>
                        <text *ngFor="let e of yearExams; index as i" [attr.x]="xFor(i)" [attr.y]="H - padB + 15" text-anchor="middle">{{ shortName(e) }}</text>
                        <ng-container *ngFor="let s of series">
                            <ng-container *ngIf="s.on">
                                <line *ngFor="let seg of s.segments" [attr.x1]="seg.x1" [attr.y1]="seg.y1" [attr.x2]="seg.x2" [attr.y2]="seg.y2"
                                    [attr.stroke]="seg.color" stroke-width="2" [attr.stroke-dasharray]="s.dashed ? '5 3' : null"/>
                                <circle *ngFor="let p of s.points" [attr.cx]="p.x" [attr.cy]="p.y" [attr.r]="s.dashed ? 3 : 4" [attr.fill]="p.color"
                                    stroke="#fff" stroke-width="1.5"><title>{{ s.name }} - {{ p.label }}: {{ p.v }}%</title></circle>
                            </ng-container>
                        </ng-container>
                        <ng-container *ngIf="view === 'overall' && series[0]?.on">
                            <text *ngFor="let p of series[0].points" class="val" [attr.x]="p.x" [attr.y]="p.y - 8" text-anchor="middle"
                                [attr.fill]="p.color">{{ p.v }}</text>
                            <!-- The mean's grade just under its point -->
                            <text *ngFor="let p of series[0].points" class="grade" [attr.x]="p.x" [attr.y]="p.y + 15" text-anchor="middle">{{ p.grade || '' }}</text>
                        </ng-container>
                    </svg>
                </div>
            </div>
        </div>

        <!-- Each exam's marks -->
        <div class="card mb-1 exam" *ngFor="let e of shown; let i = index" [class.open]="open === i" style="border-left: 4px solid #0d6efd;">
            <div class="card-header p-2 d-flex align-items-center exam-head" (click)="open = open === i ? -1 : i">
                <i class="fas fa-fw me-1" [ngClass]="open === i ? 'fa-chevron-down' : 'fa-chevron-right'"></i>
                <b class="me-2">{{ e.name }}</b>
                <small class="text-muted me-auto">{{ e.session }} {{ e.year }} - {{ e.className }}</small>
                <span class="badge bg-warning text-dark me-2" *ngIf="!e.released" title="Not yet released to parents">Provisional</span>
                <small class="text-muted me-2" *ngIf="e.classMean !== null">class {{ e.classMean }}%</small>
                <b>{{ e.mean }}%</b><small class="text-muted ms-1">mean</small>
                <span class="gr ms-2" *ngIf="e.meanGrade" [ngClass]="gradeClass(e.meanGrade)" [title]="e.meanGradeName || ''">{{ e.meanGrade }}</span>
            </div>
            <div class="card-body p-0" *ngIf="open === i">
                <table class="table table-sm mb-0 align-middle">
                    <thead class="table-light"><tr><th>Subject</th><th class="text-end">Mark</th><th class="text-end">Out of</th><th style="width: 40%;">%</th><th class="text-center" style="width: 1%;">Grade</th></tr></thead>
                    <tbody>
                        <tr *ngFor="let s of e.subjects">
                            <td>{{ s.subject }}</td>
                            <td class="text-end">{{ s.score }}</td>
                            <td class="text-end text-muted">{{ s.outOf }}</td>
                            <td><div class="d-flex align-items-center gap-2"><div class="bar flex-grow-1"><div [style.width.%]="s.percent"></div></div>
                                <span style="width: 3.5rem;" class="text-end">{{ s.percent }}%</span></div></td>
                            <td class="text-center"><span class="gr" *ngIf="s.grade" [ngClass]="gradeClass(s.grade)" [title]="s.gradeName || ''">{{ s.grade }}</span></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>`
})
export class StudentAcademicsComponent implements OnInit, AfterViewInit, OnDestroy {
    @Input() studentId!: number;
    @Input() studentName = '';
    @ViewChild('chartSvg') chartSvg?: ElementRef<SVGElement>;
    exams: ProfileExam[] = [];
    loading = false;
    open = 0;
    yearId: number | null = null;
    sessionId: number | null = null;
    view: 'overall' | 'subjects' = 'overall';
    above = ABOVE;
    below = BELOW;
    // Which report pages the user may open.
    can = {reportForms: false, assessment: false};

    // Worked out when the data or filters change (not on every redraw).
    years: {id: number; name: string}[] = [];
    yearName = '';
    sessions: {id: number; name: string}[] = [];
    /** The year's exams, oldest first: the chart's x axis. */
    yearExams: ProfileExam[] = [];
    termStarts: {x: number; name: string}[] = [];
    series: (Series & {segments: {x1: number; y1: number; x2: number; y2: number; color: string}[]})[] = [];
    hasClassMeans = false;
    /** The list: the year's (or term's) exams, newest term first, exam types in order. */
    shown: ProfileExam[] = [];

    // Chart geometry, in screen pixels: as wide as the panel, a fixed height.
    readonly H = 210;
    readonly padL = 40;
    readonly padR = 16;
    readonly padT = 26;
    readonly padB = 24;
    readonly grid = [0, 25, 50, 75, 100];
    W = 800;

    constructor(private svc: ProfilesService, private http: HttpClient, private access: AccessService,
                private host: ElementRef<HTMLElement>, private zone: NgZone) {}

    // The chart follows the panel's width: when the tab opens, and when the window resizes.
    private resize?: ResizeObserver;
    private lastWidth = 0;

    ngOnInit(): void {
        this.loading = true;
        this.access.canAny('/reports/academics/exam-results', '/reports/academics/report-forms').subscribe((ok) => (this.can.reportForms = ok));
        this.access.canAny('/reports/academics/assessment-report').subscribe((ok) => (this.can.assessment = ok));
        forkJoin([this.svc.studentAcademics(this.studentId), this.http.get<any[]>('/academicYears')]).subscribe({
            next: ([exams, years]) => {
                this.loading = false;
                this.exams = exams;
                // The current academic year if the student sat exams in it, else their latest.
                const current = (years || []).find((y) => y.status === true);
                const have = new Set(exams.map((e) => e.academicYearId));
                this.yearId = current && have.has(current.id) ? current.id : exams[0]?.academicYearId ?? null;
                this.refresh();
            },
            error: () => (this.loading = false)
        });
    }

    ngAfterViewInit(): void {
        this.resize = new ResizeObserver(() => {
            const w = this.host.nativeElement.clientWidth;
            if (w && Math.abs(w - this.lastWidth) > 4) this.zone.run(() => this.layout());
        });
        this.resize.observe(this.host.nativeElement);
    }

    ngOnDestroy(): void {
        this.resize?.disconnect();
        removePrintFrame();
    }

    setView(v: 'overall' | 'subjects') {
        this.view = v;
        this.buildSeries();
    }

    /** The year changed (or first load): the chart, the terms and the list. */
    refresh() {
        const years = new Map<number, {id: number; name: string; start: string}>();
        this.exams.forEach((e) => {
            const y = years.get(e.academicYearId);
            if (!y || e.sessionStart < y.start) years.set(e.academicYearId, {id: e.academicYearId, name: e.year, start: e.sessionStart});
        });
        this.years = [...years.values()].sort((a, b) => b.start.localeCompare(a.start));
        this.yearName = this.years.find((y) => y.id === this.yearId)?.name || '';

        this.yearExams = this.exams.filter((e) => e.academicYearId === this.yearId)
            .sort((a, b) => a.sessionStart.localeCompare(b.sessionStart) || a.examTypeRank - b.examTypeRank || a.startDate.localeCompare(b.startDate));
        const terms = new Map<number, {id: number; name: string}>();
        this.yearExams.forEach((e) => terms.set(e.sessionId, {id: e.sessionId, name: e.session}));
        this.sessions = [...terms.values()];
        this.layout();
        this.refreshList();
    }

    /** The term changed: only the list below the chart. */
    refreshList() {
        this.open = 0;
        this.shown = this.exams.filter((e) => e.academicYearId === this.yearId && (this.sessionId === null || e.sessionId === this.sessionId));
    }

    // Fits the chart to the panel (at least about 70px an exam), then places everything on it.
    private layout() {
        this.lastWidth = this.host.nativeElement.clientWidth;
        const panel = (this.lastWidth || 900) - 40;
        this.W = Math.max(600, panel, this.padL + this.padR + 50 + this.yearExams.length * 70);
        this.termStarts = this.yearExams
            .map((e, i) => ({e, i}))
            .filter(({e, i}) => i === 0 || this.yearExams[i - 1].sessionId !== e.sessionId)
            .map(({e, i}) => ({x: i === 0 ? this.padL + 2 : (this.xFor(i - 1) + this.xFor(i)) / 2, name: e.session}));
        this.buildSeries();
    }

    private buildSeries() {
        const label = (e: ProfileExam) => `${e.name}, ${e.session}`;
        const keep = new Map(this.series.map((s) => [s.name, s.on]));
        let series: Series[];
        if (this.view === 'overall') {
            const mine = this.yearExams.map((e, i) => ({
                x: this.xFor(i), y: this.yFor(e.mean), v: e.mean, label: label(e) + (e.meanGrade ? ` (${e.meanGrade})` : ''),
                color: e.classMean === null ? NEUTRAL : e.mean >= e.classMean ? ABOVE : BELOW,
                grade: e.meanGrade
            }));
            const cls = this.yearExams.map((e, i) => ({e, i})).filter(({e}) => e.classMean !== null)
                .map(({e, i}) => ({x: this.xFor(i), y: this.yFor(e.classMean!), v: e.classMean!, label: label(e), color: '#6c757d'}));
            series = [{name: 'This student (mean)', color: NEUTRAL, dashed: false, points: mine, on: true}];
            if (cls.length) series.push({name: 'Class mean', color: '#6c757d', dashed: true, points: cls, on: true});
        } else {
            // One line per subject, in the subjects' set order.
            const ranks = new Map<string, number>();
            this.yearExams.forEach((e) => e.subjects.forEach((s) => ranks.set(s.subject, s.subjectRank)));
            const order = [...ranks.keys()].sort((a, b) => ranks.get(a)! - ranks.get(b)! || a.localeCompare(b));
            series = order.map((name, k) => {
                const color = SLOTS[k] ?? OTHER;
                return {
                    name,
                    color,
                    dashed: false,
                    // Past eight subjects the colours run out: those start hidden, to be shown one at a time.
                    on: keep.get(name) ?? k < SLOTS.length,
                    points: this.yearExams.map((e, i) => ({e, i, s: e.subjects.find((x) => x.subject === name)}))
                        .filter(({s}) => !!s)
                        .map(({e, i, s}) => ({x: this.xFor(i), y: this.yFor(s!.percent), v: s!.percent, label: label(e), color}))
                };
            });
        }
        // Line pieces between points, each in its end point's colour: the student's line
        // turns green or red as it moves above or below the class mean.
        this.series = series.map((s) => ({
            ...s,
            segments: s.points.slice(1).map((p, i) => ({x1: s.points[i].x, y1: s.points[i].y, x2: p.x, y2: p.y, color: s.dashed ? s.color : p.color}))
        }));
        this.hasClassMeans = this.series.some((s) => s.dashed);
    }

    xFor(i: number): number {
        const n = this.yearExams.length;
        const span = this.W - this.padL - this.padR - 50;
        return this.padL + 25 + (n <= 1 ? span / 2 : (span * i) / (n - 1));
    }

    yFor(v: number): number {
        const h = this.H - this.padT - this.padB;
        return this.padT + h - (Math.max(0, Math.min(100, v)) / 100) * h;
    }

    // EE1/EE2 -> exceeding, ME -> meeting, AE -> approaching, BE -> below expectations.
    gradeClass(grade: string | null): string {
        const g = (grade || '').toUpperCase();
        return g.startsWith('EE') ? 'gr-ee' : g.startsWith('ME') ? 'gr-me' : g.startsWith('AE') ? 'gr-ae' : g.startsWith('BE') ? 'gr-be' : '';
    }

    // "End-Term Exam" -> "End-Term", to fit under the chart.
    shortName(e: ProfileExam): string {
        return e.name.replace(/\s+exam\b/i, '').split(' - ')[0];
    }

    /** The term the report form and assessment report print for: the chosen one, else the year's latest. */
    get printTerm(): ProfileExam | null {
        const inTerm = this.sessionId === null ? this.yearExams : this.yearExams.filter((e) => e.sessionId === this.sessionId);
        return inTerm[inTerm.length - 1] ?? null;
    }

    // The report page's filters, so it opens with this student chosen.
    link(tab: string | null, withTerm: boolean): any {
        const e = withTerm ? this.printTerm : this.yearExams[this.yearExams.length - 1];
        if (!e) return {};
        const q: any = {curriculumId: e.curriculumId, academicYearId: e.academicYearId, schoolClassId: e.schoolClassId, studentId: this.studentId};
        if (withTerm) q.sessionId = e.sessionId;
        if (tab) q.tab = tab;
        return q;
    }

    // The year's progress: the chart as it is on screen, and each exam against the class.
    printProgress() {
        const chart = this.chartSvg?.nativeElement.outerHTML || '';
        const rows = this.yearExams.map((e) => {
            const diff = e.classMean === null ? null : Math.round((e.mean - e.classMean) * 10) / 10;
            return `<tr><td>${escapeHtml(e.session)}</td><td>${escapeHtml(e.name)}</td><td class="n">${e.mean}%</td><td>${escapeHtml(e.meanGrade || '')}</td>
                <td class="n">${e.classMean === null ? '-' : e.classMean + '%'}</td>
                <td class="n ${diff === null ? '' : diff >= 0 ? 'up' : 'down'}">${diff === null ? '-' : (diff > 0 ? '+' : '') + diff}</td></tr>`;
        }).join('');
        const cls = this.yearExams[this.yearExams.length - 1]?.className || '';
        printInFrame(`Progress report - ${this.studentName}`, PRINT_CSS,
            `<h1>${escapeHtml(this.studentName)} - progress through ${escapeHtml(this.yearName)}</h1>
             <div class="sub">${escapeHtml(cls)}. Mean mark in each exam against the class mean (green: at or above, red: below).</div>
             ${chart}
             <table><thead><tr><th>Term</th><th>Exam</th><th>Mean</th><th>Grade</th><th>Class mean</th><th>Against the class</th></tr></thead>
             <tbody>${rows}</tbody></table>`);
    }
}
