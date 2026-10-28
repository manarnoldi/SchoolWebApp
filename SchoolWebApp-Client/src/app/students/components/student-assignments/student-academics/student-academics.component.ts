import {Component, Input, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {forkJoin} from 'rxjs';
import {ProfileExam, ProfilesService} from '@/core/services/profiles.service';

interface ChartPoint {
    x: number;
    y: number;
    v: number;
    label: string;
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

/**
 * Student page, Academics tab: for a year (the current one first), a progress
 * chart across the whole year - the student's mean against the class mean, or
 * subject by subject - then each exam's marks, filtered by term: terms newest
 * first, exams in exam-type order, subjects in their set order.
 */
@Component({
    selector: 'app-student-academics',
    styles: [`
        .exam-head { cursor: pointer; background: #e7f1ff; color: #084298; }
        .exam-head:hover { background: #dbe9ff; }
        .exam.open > .exam-head { background: #0d6efd; color: #fff; }
        .exam.open > .exam-head .text-muted { color: #e7f1ff !important; }
        .bar { height: 8px; background: #e9ecef; border-radius: 4px; overflow: hidden; min-width: 80px; }
        .bar > div { height: 100%; background: #2a78d6; }
        .chart { width: 100%; height: auto; display: block; }
        .chart text { font-size: 11px; fill: #52514e; }
        .chart .val { font-size: 11px; font-weight: 600; fill: #0b0b0b; }
        .key { display: inline-flex; align-items: center; gap: 6px; margin: 2px 14px 2px 0; font-size: .8rem; color: #212529; }
        .key.toggle { cursor: pointer; user-select: none; }
        .key.off { opacity: .35; }
        .sw { width: 12px; height: 12px; border-radius: 3px; display: inline-block; }
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
                <div class="mb-1">
                    <span *ngFor="let s of series" class="key" [class.toggle]="view === 'subjects'" [class.off]="!s.on"
                        (click)="view === 'subjects' && (s.on = !s.on)" [title]="view === 'subjects' ? 'Show or hide ' + s.name : ''">
                        <svg width="22" height="8" *ngIf="view === 'overall'"><line x1="0" y1="4" x2="22" y2="4" [attr.stroke]="s.color" stroke-width="2"
                            [attr.stroke-dasharray]="s.dashed ? '5 3' : null"/></svg>
                        <span class="sw" *ngIf="view === 'subjects'" [style.background]="s.color"></span>{{ s.name }}</span>
                    <small class="text-muted" *ngIf="view === 'subjects'">- click a subject to show or hide it</small>
                </div>
                <!-- A long year scrolls sideways rather than shrinking -->
                <div style="overflow-x: auto;">
                <svg class="chart" [style.min-width.px]="W > 1100 ? W : null" [attr.viewBox]="'0 0 ' + W + ' ' + H" role="img" [attr.aria-label]="'Marks through ' + yearName">
                    <g *ngFor="let g of grid">
                        <line [attr.x1]="padL" [attr.x2]="W - padR" [attr.y1]="yFor(g)" [attr.y2]="yFor(g)" stroke="#e9ecef"/>
                        <text [attr.x]="padL - 6" [attr.y]="yFor(g) + 4" text-anchor="end">{{ g }}%</text>
                    </g>
                    <!-- Term bands: a faint divider where each term starts -->
                    <g *ngFor="let t of termStarts">
                        <line [attr.x1]="t.x" [attr.x2]="t.x" [attr.y1]="padT - 8" [attr.y2]="H - padB" stroke="#cfe2ff" stroke-dasharray="3 3"/>
                        <text [attr.x]="t.x + 4" [attr.y]="padT - 10" style="font-weight: 600; fill: #084298;">{{ t.name }}</text>
                    </g>
                    <text *ngFor="let e of yearExams; index as i" [attr.x]="xFor(i)" [attr.y]="H - padB + 16" text-anchor="middle">{{ shortName(e) }}</text>
                    <ng-container *ngFor="let s of series">
                        <ng-container *ngIf="s.on">
                            <polyline [attr.points]="line(s.points)" fill="none" [attr.stroke]="s.color" stroke-width="2"
                                [attr.stroke-dasharray]="s.dashed ? '5 3' : null"/>
                            <circle *ngFor="let p of s.points" [attr.cx]="p.x" [attr.cy]="p.y" [attr.r]="s.dashed ? 3.5 : 4.5" [attr.fill]="s.color"
                                stroke="#fff" stroke-width="1.5"><title>{{ s.name }} - {{ p.label }}: {{ p.v }}%</title></circle>
                        </ng-container>
                    </ng-container>
                    <!-- The student's mean, labelled (overall view) -->
                    <ng-container *ngIf="view === 'overall' && series[0]?.on">
                        <text *ngFor="let p of series[0].points" class="val" [attr.x]="p.x" [attr.y]="p.y - 10" text-anchor="middle">{{ p.v }}</text>
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
            </div>
            <div class="card-body p-0" *ngIf="open === i">
                <table class="table table-sm mb-0 align-middle">
                    <thead class="table-light"><tr><th>Subject</th><th class="text-end">Mark</th><th class="text-end">Out of</th><th style="width: 40%;">%</th></tr></thead>
                    <tbody>
                        <tr *ngFor="let s of e.subjects">
                            <td>{{ s.subject }}</td>
                            <td class="text-end">{{ s.score }}</td>
                            <td class="text-end text-muted">{{ s.outOf }}</td>
                            <td><div class="d-flex align-items-center gap-2"><div class="bar flex-grow-1"><div [style.width.%]="s.percent"></div></div>
                                <span style="width: 3.5rem;" class="text-end">{{ s.percent }}%</span></div></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>`
})
export class StudentAcademicsComponent implements OnInit {
    @Input() studentId!: number;
    exams: ProfileExam[] = [];
    loading = false;
    open = 0;
    yearId: number | null = null;
    sessionId: number | null = null;
    view: 'overall' | 'subjects' = 'overall';

    // Worked out when the data or filters change (not on every redraw).
    years: {id: number; name: string}[] = [];
    yearName = '';
    sessions: {id: number; name: string}[] = [];
    /** The year's exams, oldest first: the chart's x axis. */
    yearExams: ProfileExam[] = [];
    termStarts: {x: number; name: string}[] = [];
    series: Series[] = [];
    /** The list: the year's (or term's) exams, newest term first, exam types in order. */
    shown: ProfileExam[] = [];

    // Chart geometry (viewBox units); it widens with the number of exams.
    readonly H = 300;
    readonly padL = 44;
    readonly padR = 24;
    readonly padT = 34;
    readonly padB = 34;
    readonly grid = [0, 25, 50, 75, 100];
    W = 1000;

    constructor(private svc: ProfilesService, private http: HttpClient) {}

    ngOnInit(): void {
        this.loading = true;
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

        // About 90 units per exam, so a full year of exams stays readable across the page.
        this.W = Math.max(760, this.padL + this.padR + 40 + this.yearExams.length * 90);
        this.termStarts = this.yearExams
            .map((e, i) => ({e, i}))
            .filter(({e, i}) => i === 0 || this.yearExams[i - 1].sessionId !== e.sessionId)
            .map(({e, i}) => ({x: this.xFor(i) - (i === 0 ? 20 : 45), name: e.session}));
        this.buildSeries();
        this.refreshList();
    }

    /** The term changed: only the list below the chart. */
    refreshList() {
        this.open = 0;
        this.shown = this.exams.filter((e) => e.academicYearId === this.yearId && (this.sessionId === null || e.sessionId === this.sessionId));
    }

    private buildSeries() {
        const label = (e: ProfileExam) => `${e.name}, ${e.session}`;
        if (this.view === 'overall') {
            const mine = this.yearExams.map((e, i) => ({x: this.xFor(i), y: this.yFor(e.mean), v: e.mean, label: label(e)}));
            const cls = this.yearExams.map((e, i) => ({e, i})).filter(({e}) => e.classMean !== null)
                .map(({e, i}) => ({x: this.xFor(i), y: this.yFor(e.classMean!), v: e.classMean!, label: label(e)}));
            this.series = [{name: 'This student (mean)', color: '#2a78d6', dashed: false, points: mine, on: true}];
            if (cls.length) this.series.push({name: 'Class mean', color: '#6c757d', dashed: true, points: cls, on: true});
            return;
        }
        // One line per subject, in the subjects' set order.
        const ranks = new Map<string, number>();
        this.yearExams.forEach((e) => e.subjects.forEach((s) => ranks.set(s.subject, s.subjectRank)));
        const order = [...ranks.keys()].sort((a, b) => ranks.get(a)! - ranks.get(b)! || a.localeCompare(b));
        this.series = order.map((name, k) => ({
            name,
            color: SLOTS[k] ?? OTHER,
            dashed: false,
            // Past eight subjects the colours run out: those start hidden, to be shown one at a time.
            on: k < SLOTS.length,
            points: this.yearExams.map((e, i) => ({e, i, s: e.subjects.find((x) => x.subject === name)}))
                .filter(({s}) => !!s)
                .map(({e, i, s}) => ({x: this.xFor(i), y: this.yFor(s!.percent), v: s!.percent, label: label(e)}))
        }));
    }

    xFor(i: number): number {
        const n = this.yearExams.length;
        const span = this.W - this.padL - this.padR - 40;
        return this.padL + 20 + (n <= 1 ? span / 2 : (span * i) / (n - 1));
    }

    yFor(v: number): number {
        const h = this.H - this.padT - this.padB;
        return this.padT + h - (Math.max(0, Math.min(100, v)) / 100) * h;
    }

    line(points: ChartPoint[]): string {
        return points.map((p) => `${p.x},${p.y}`).join(' ');
    }

    // "End-Term Exam" -> "End-Term", to fit under the chart.
    shortName(e: ProfileExam): string {
        return e.name.replace(/\s+exam\b/i, '').split(' - ')[0];
    }
}
