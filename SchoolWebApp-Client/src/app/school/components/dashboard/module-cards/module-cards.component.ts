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
        .meter { height: 10px; background: #e7f1ff; border-radius: 5px; overflow: hidden; }
        .meter > div { height: 100%; background: #2a78d6; border-radius: 5px; }
        .stat .v { font-size: 1.25rem; font-weight: 700; line-height: 1.1; font-variant-numeric: tabular-nums; }
        .stat .k { font-size: .75rem; color: #52514e; text-transform: uppercase; letter-spacing: .04em; }
        .card-title-sm { font-size: .95rem; font-weight: 600; }
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
                this.http.get('/dashboardCards/finance').subscribe({next: (d) => (this.finance = d), error: () => (this.show.finance = false)}));
            this.card('Payroll', cardPath('payroll'), 'payroll', () =>
                this.http.get('/dashboardCards/payroll').subscribe({next: (d) => (this.payroll = d), error: () => (this.show.payroll = false)}));
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
            },
            error: () => (this.show.teaching = false)
        });
    }

    pct(n: number, of: number): number {
        return of ? Math.round((100 * n) / of) : 0;
    }

    // "Net pay up 2.5% on September 2026", or nothing without an earlier payroll.
    change(now: number, before: number): string {
        if (!before || !this.payroll?.previous) return '';
        const pct = Math.round(((now - before) / before) * 1000) / 10;
        if (pct === 0) return `Net pay the same as ${this.payroll.previous.name}`;
        return `Net pay ${pct > 0 ? 'up' : 'down'} ${Math.abs(pct)}% on ${this.payroll.previous.name}`;
    }
}
