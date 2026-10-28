import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {forkJoin, of, switchMap} from 'rxjs';
import {LicenceService} from '@/core/services/licence.service';
import {AccessService} from '@/core/services/access.service';
import {TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {TrackerRow} from '@/teaching/models/teaching-models';
import {STAGE_BADGES, STAGE_LABELS} from '@/elections/models/election-models';
import {cardPath} from '@/core/models/dashboard-cards';

interface TeachingSummary {
    term: string;
    allocations: number;
    schemesApproved: number;
    schemesOverdue: number;
    weeksApproved: number;
    weeksDue: number;
    lessonsTaught: number;
    lessonsPlanned: number;
}

/**
 * The dashboard's module cards - fees, payroll, teaching, elections, messages.
 * A card shows when the school's licence has its module and the user may open
 * the pages it summarises (the API checks both again).
 */
@Component({
    selector: 'app-dashboard-module-cards',
    styles: [`
        .meter { height: 8px; background: #e9ecef; border-radius: 4px; overflow: hidden; margin: 2px 0 8px; }
        .meter > div { height: 100%; background: #2a78d6; border-radius: 4px; transition: width .6s; }
        .card-title-sm { font-size: .95rem; font-weight: 600; }
        /* Cards in a row that is not full stretch to share it, so no gap is left. */
        .row > .grow { flex-grow: 1; }
        .big { font-size: 1.45rem; font-weight: 700; line-height: 1.1; font-variant-numeric: tabular-nums; }
        /* Tinted figure tiles */
        .tile { border-radius: 8px; padding: 6px 8px; position: relative; height: 100%; }
        .tile > i { position: absolute; top: 7px; right: 8px; opacity: .45; }
        .tile .v { font-size: 1.1rem; font-weight: 700; line-height: 1.2; font-variant-numeric: tabular-nums; }
        .tile .k { font-size: .7rem; text-transform: uppercase; letter-spacing: .04em; opacity: .85; }
        /* Fees: collection ring, arrears bars */
        .ring { width: 84px; height: 84px; border-radius: 50%; display: grid; place-items: center; flex: none; }
        .ring > div { width: 64px; height: 64px; border-radius: 50%; background: #fff; display: flex; flex-direction: column;
            align-items: center; justify-content: center; line-height: 1.1; }
        .ring b { font-size: 1.05rem; } .ring span { font-size: .65rem; color: #6c757d; }
        .arrear { display: grid; grid-template-columns: minmax(0, 1.3fr) 1fr auto; gap: 8px; align-items: center; padding: 3px 0; border-top: 1px solid #f1f3f5; }
        .arrear .name { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
        .arrear .bar { height: 6px; background: #f8d7da; border-radius: 3px; overflow: hidden; }
        .arrear .bar > div { height: 100%; background: #b02a37; border-radius: 3px; }
        /* Payroll: run stages, gross split, net trend */
        .steps { display: flex; flex-wrap: wrap; gap: 3px; }
        .steps .badge { font-size: .68rem; }
        .split { display: flex; gap: 2px; height: 14px; border-radius: 4px; overflow: hidden; }
        .split > div { min-width: 3px; }
        .dot { display: inline-block; width: 8px; height: 8px; border-radius: 50%; margin-right: 4px; }
        .trend { display: flex; align-items: flex-end; gap: 4px; height: 64px; }
        .trend .col { flex: 1; display: flex; flex-direction: column; justify-content: flex-end; height: 100%; padding: 0; }
        .trend .bar { background: #b6d4fe; border-radius: 4px 4px 0 0; min-height: 2px; }
        .trend .bar.now { background: #0dcaf0; }
        .trend span { font-size: .65rem; color: #6c757d; text-align: center; }
        .prog .badge { font-size: .7rem; }
        .election { padding: 4px 0; }
    `],
    templateUrl: './module-cards.component.html'
})
export class ModuleCardsComponent implements OnInit {
    show = {finance: false, payroll: false, teaching: false, elections: false, communications: false};
    finance: any = null;
    payroll: any = null;
    teaching: TeachingSummary | null = null;
    elections: any[] | null = null;
    communications: any = null;
    abs = Math.abs;
    payrollSteps = ['Draft', 'Processed', 'Approved', 'Posted'];
    // Gross pay split, in this order (it keeps neighbouring colours apart for colour-blind readers).
    payrollSplit: {label: string; amount: number; fill: string}[] = [];
    teachingMeters: {label: string; icon: string; pct: number; text: string}[] = [];
    maxArrears = 0;
    maxTrend = 0;
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;

    constructor(private http: HttpClient, private licence: LicenceService, private access: AccessService,
                private teachingSvc: TeachingDocumentsService) {}

    get any(): boolean {
        return Object.values(this.show).some((v) => v);
    }

    ngOnInit(): void {
        this.licence.load().subscribe(() => {
            this.card('Finance', cardPath('finance'), 'finance', () =>
                this.http.get('/dashboardCards/finance').subscribe({next: (d: any) => {
                    this.finance = d;
                    this.maxArrears = Math.max(0, ...d.arrearsByClass.map((a: any) => +a.balance));
                }, error: () => (this.show.finance = false)}));
            this.card('Payroll', cardPath('payroll'), 'payroll', () =>
                this.http.get('/dashboardCards/payroll').subscribe({next: (d) => this.setPayroll(d), error: () => (this.show.payroll = false)}));
            this.card('Academics', cardPath('teaching'), 'teaching', () => this.loadTeaching());
            this.card('Elections', cardPath('elections'), 'elections', () =>
                this.http.get<any[]>('/dashboardCards/elections').subscribe({
                    next: (d) => {
                        this.elections = d;
                        if (d.length === 0) this.show.elections = false;
                    },
                    error: () => (this.show.elections = false)
                }));
            this.card('Communications', cardPath('communications'), 'communications', () =>
                this.http.get('/dashboardCards/communications').subscribe({next: (d) => (this.communications = d), error: () => (this.show.communications = false)}));
        });
    }

    // Shows a card (and loads it) when its module is licensed and the user's role holds the card
    // (Security > Menu Permissions > Dashboard > Cards).
    private card(module: any, path: string, key: keyof ModuleCardsComponent['show'], load: () => void) {
        if (!this.licence.has(module)) return;
        this.access.canAny(path).subscribe((ok) => {
            this.show[key] = ok;
            if (ok) load();
        });
    }

    // Teaching: the current term's tracker, as far as the user oversees it, summed up.
    private loadTeaching() {
        this.teachingSvc.context().pipe(
            switchMap((c) => forkJoin([of(c), c.currentSessionId ? this.teachingSvc.tracker({sessionId: c.currentSessionId} as any) : of(null)]))
        ).subscribe({
            next: ([c, t]) => {
                if (!t) {
                    this.show.teaching = false;
                    return;
                }
                const rows: TrackerRow[] = t.rows;
                const dueWeeks = (r: TrackerRow) => r.weeks.filter((w, i) => w.status !== 'NotApplicable' && t.weeks[i] && new Date(t.weeks[i].dueAt) <= new Date());
                this.teaching = {
                    term: `${t.session.name} ${t.session.academicYearName}`,
                    allocations: rows.length,
                    schemesApproved: rows.filter((r) => r.scheme.status === 'Approved').length,
                    schemesOverdue: rows.filter((r) => r.scheme.overdue).length,
                    weeksApproved: rows.reduce((n, r) => n + dueWeeks(r).filter((w) => w.status === 'Approved').length, 0),
                    weeksDue: rows.reduce((n, r) => n + dueWeeks(r).length, 0),
                    lessonsTaught: rows.reduce((n, r) => n + r.taughtLessons, 0),
                    lessonsPlanned: rows.reduce((n, r) => n + r.plannedLessons, 0)
                };
                const tm = this.teaching;
                this.teachingMeters = [
                    {label: 'Schemes approved', icon: 'fa-book', pct: this.pct(tm.schemesApproved, tm.allocations), text: `${tm.schemesApproved} / ${tm.allocations}`},
                    {label: 'Lesson plans (weeks due)', icon: 'fa-clipboard-list', pct: this.pct(tm.weeksApproved, tm.weeksDue), text: `${this.pct(tm.weeksApproved, tm.weeksDue)}%`},
                    {label: 'Lessons taught', icon: 'fa-chalkboard', pct: this.pct(tm.lessonsTaught, tm.lessonsPlanned), text: `${tm.lessonsTaught} / ${tm.lessonsPlanned}`}
                ];
            },
            error: () => (this.show.teaching = false)
        });
    }

    pct(n: number, of: number): number {
        return of ? Math.round((100 * n) / of) : 0;
    }

    private setPayroll(d: any) {
        this.payroll = d;
        const p = d?.latest;
        this.maxTrend = Math.max(0, ...(d?.trend || []).map((t: any) => +t.net));
        if (!p) return;
        const other = Math.max(0, p.gross - p.net - p.paye - p.nssf - p.shif - p.ahl);
        this.payrollSplit = [
            {label: 'Net', amount: p.net, fill: '#1f7a4d'},
            {label: 'PAYE', amount: p.paye, fill: '#2a78d6'},
            {label: 'NSSF', amount: p.nssf, fill: '#c8641a'},
            {label: 'SHIF', amount: p.shif, fill: '#7c4dbd'},
            {label: 'Housing levy', amount: p.ahl, fill: '#1aa3a3'},
            {label: 'Other', amount: other, fill: 'repeating-linear-gradient(45deg, #adb5bd 0 3px, #dee2e6 3px 6px)'}
        ].filter((x) => x.amount > 0);
    }

    // Net pay against the payroll before: "up 2.5% vs September 2026".
    netChange(): {pct: number; name: string} | null {
        const now = this.payroll?.latest?.net, before = this.payroll?.previous?.net;
        if (!before) return null;
        return {pct: Math.round(((now - before) / before) * 1000) / 10, name: this.payroll.previous.name};
    }

    stepIndex(status: string): number {
        return this.payrollSteps.indexOf(status);
    }

    monthShort(m: number): string {
        return new Date(2000, (m || 1) - 1, 1).toLocaleString('en-GB', {month: 'short'});
    }

    // A conic ring filled to pct.
    ring(pct: number, colour: string): string {
        return `conic-gradient(${colour} ${pct * 3.6}deg, #e9ecef 0)`;
    }

    // Progress colour: green from 75%, amber from 40%, red below.
    levelColour(pct: number): string {
        return pct >= 75 ? 'success' : pct >= 40 ? 'warning' : 'danger';
    }

    level(pct: number): string {
        const c = this.levelColour(pct);
        return `bg-${c}-subtle text-${c}-emphasis`;
    }
}
