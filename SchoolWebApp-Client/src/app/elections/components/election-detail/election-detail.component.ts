import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {filter, Subscription} from 'rxjs';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {
    apiError,
    ElectionDetail,
    ElectionStage,
    STAGE_ACTIONS,
    STAGE_BADGES,
    STAGE_LABELS,
    VoterRegisterMode
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';
import {ElectionContext} from '@/elections/services/election-context.service';

// What moving to a stage means, shown before the move is confirmed.
const STAGE_CONFIRM: {[s: number]: string} = {
    [ElectionStage.Draft]: 'Nominations close and the election goes back to set-up.',
    [ElectionStage.Nominations]: 'Candidates can now be nominated for each race. Positions can still be changed until nominations close.',
    [ElectionStage.CandidatesFinal]: 'The approved candidates become final and are put on the ballot in alphabetical order. '
        + 'With a frozen register, the voter register is taken now. Positions can no longer change.',
    [ElectionStage.Polling]: 'Returning officers can issue ballots at their stations. Results show live on the results page.',
    [ElectionStage.PollingClosed]: 'No more ballots can be issued. You can reopen polling until the results are declared.',
    [ElectionStage.Declared]: 'The leading candidates are declared elected and the results become final and visible to all staff.'
};
const REOPEN_POLLING_CONFIRM = 'Returning officers can issue ballots again.';
const REOPEN_CONFIRM = 'Nominations open again. The ballot order is cleared and a frozen voter register is discarded; '
    + 'both are taken again when nominations close.';

interface ElectionTab {
    path: string;
    label: string;
    icon: string;
    /** The stage the tab opens at; open from the start when unset. */
    opensAt?: ElectionStage;
    closedNote?: string;
    manageOnly?: boolean;
}

// In the order an election runs.
const TABS: ElectionTab[] = [
    {path: 'positions', label: 'Positions', icon: 'fa-user-tie'},
    {path: 'nominations', label: 'Nominations', icon: 'fa-user-check', opensAt: ElectionStage.Nominations, closedNote: 'Opens when nominations open'},
    {path: 'register', label: 'Voter register', icon: 'fa-clipboard-list', opensAt: ElectionStage.Nominations, closedNote: 'Opens when nominations open'},
    {path: 'stations', label: 'Polling stations', icon: 'fa-person-booth', opensAt: ElectionStage.Nominations, closedNote: 'Opens when nominations open'},
    {path: 'results', label: 'Results', icon: 'fa-poll', opensAt: ElectionStage.Polling, closedNote: 'Opens when polling opens'},
    {path: 'leadership', label: 'Leadership', icon: 'fa-id-badge', opensAt: ElectionStage.Declared, closedNote: 'Opens when the results are declared', manageOnly: true},
    {path: 'history', label: 'History', icon: 'fa-history'}
];

/**
 * One election: its details, stage and next step, with a tab for each part of
 * running it (positions, nominations, register, stations, results, leadership).
 */
@Component({
    selector: 'app-election-detail',
    templateUrl: './election-detail.component.html',
    styles: [`
        .nav-tabs .nav-link { padding: .4rem .8rem; }
        .nav-tabs .nav-link.disabled { color: #adb5bd; }
    `],
    providers: [ElectionContext]
})
export class ElectionDetailComponent implements OnInit, OnDestroy {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'},
        {link: [], title: 'Election'}
    ];
    dashboardTitle = 'Elections: Election';

    election: ElectionDetail | null = null;
    loading = false;
    busy = false;
    showEdit = false;
    /** Bumped to rebuild the open tab, so it reloads after a stage move. */
    outletKey = 0;

    steps = [ElectionStage.Draft, ElectionStage.Nominations, ElectionStage.CandidatesFinal,
        ElectionStage.Polling, ElectionStage.PollingClosed, ElectionStage.Declared];
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    stageActions = STAGE_ACTIONS;
    Mode = VoterRegisterMode;
    Stage = ElectionStage;

    private subs: Subscription[] = [];

    constructor(private svc: ElectionsService, private toastr: ToastrService,
                private route: ActivatedRoute, public router: Router, public ctx: ElectionContext) {}

    ngOnInit(): void {
        this.svc.options().subscribe({
            next: (o) => this.ctx.options$.next(o),
            error: (err) => this.toastr.error(apiError(err, 'Error loading the election options.'))
        });
        this.subs.push(
            this.route.paramMap.subscribe((p) => this.load(Number(p.get('id')))),
            this.ctx.reload$.subscribe(() => this.election && this.load(this.election.id)),
            // A tab changing the election (e.g. a position saved) updates the header too.
            this.ctx.election$.subscribe((d) => d && this.show(d)),
            // Opening the election without a tab goes to the tab for its stage.
            this.router.events.pipe(filter((ev) => ev instanceof NavigationEnd)).subscribe(() => this.openDefaultTab())
        );
    }

    ngOnDestroy(): void {
        this.subs.forEach((s) => s.unsubscribe());
    }

    load(id: number) {
        this.loading = true;
        this.svc.get(id).subscribe({
            next: (d) => {
                this.loading = false;
                this.ctx.election$.next(d);
                this.openDefaultTab();
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.status === 404 ? 'That election no longer exists.' : apiError(err, 'Error loading the election.'));
                if (err.status === 404) this.router.navigate(['/elections/manage']);
            }
        });
    }

    show(d: ElectionDetail) {
        this.election = d;
        const tab = this.tabs.find((t) => t.path === this.currentTab);
        this.breadcrumbs = [...this.breadcrumbs.slice(0, 2), {link: ['/elections/manage', String(d.id)], title: d.name}];
        if (tab) this.breadcrumbs.push({link: ['/elections/manage', String(d.id), tab.path], title: tab.label});
        this.dashboardTitle = 'Elections: ' + d.name;
    }

    get canManage(): boolean {
        return this.ctx.canManage;
    }

    get tabs(): ElectionTab[] {
        return TABS.filter((t) => !t.manageOnly || this.canManage);
    }

    get currentTab(): string {
        return this.route.firstChild?.routeConfig?.path || '';
    }

    isOpen(t: ElectionTab): boolean {
        const e = this.election;
        if (!e || t.opensAt === undefined) return true;
        // Nominations taken before a move back to draft stay viewable.
        if (t.path === 'nominations' && (e.candidates || e.pendingNominations)) return true;
        return e.stage >= t.opensAt;
    }

    // Where an election in each stage is worked on.
    defaultTab(stage: ElectionStage): string {
        switch (stage) {
            case ElectionStage.Draft: return 'positions';
            case ElectionStage.Nominations: return 'nominations';
            case ElectionStage.CandidatesFinal: return 'stations';
            default: return 'results';
        }
    }

    openDefaultTab() {
        if (!this.election || this.currentTab) {
            if (this.election) this.show(this.election);
            return;
        }
        this.router.navigate([this.defaultTab(this.election.stage)], {relativeTo: this.route, replaceUrl: true});
    }

    get forwardStages(): ElectionStage[] {
        return this.election?.nextStages.filter((s) => s > this.election!.stage) || [];
    }

    get backStages(): ElectionStage[] {
        return this.election?.nextStages.filter((s) => s < this.election!.stage) || [];
    }

    // Moving back from Candidates final reopens nominations.
    actionLabel(to: ElectionStage): string {
        if (to === ElectionStage.Nominations && this.election?.stage === ElectionStage.CandidatesFinal) return 'Reopen nominations';
        if (to === ElectionStage.Polling && this.election?.stage === ElectionStage.PollingClosed) return 'Reopen polling';
        return this.stageActions[to];
    }

    stepDone(s: ElectionStage): boolean {
        return !!this.election && s < this.election.stage;
    }

    changeStage(to: ElectionStage) {
        const e = this.election!;
        Swal.fire({
            title: `${this.actionLabel(to)}?`,
            text: to < e.stage && to === ElectionStage.Nominations ? REOPEN_CONFIRM
                : to < e.stage && to === ElectionStage.Polling ? REOPEN_POLLING_CONFIRM : STAGE_CONFIRM[to] || '',
            input: 'text',
            inputPlaceholder: 'Note for the record (optional)',
            width: 460, position: 'top', icon: to < e.stage ? 'warning' : 'question',
            showCancelButton: true, confirmButtonText: this.actionLabel(to)
        }).then((r) => {
            if (!r.isConfirmed) return;
            this.busy = true;
            this.svc.changeStage(e.id, to, r.value || null).subscribe({
                next: (d) => {
                    this.busy = false;
                    this.toastr.success(`${d.name}: ${this.stageLabels[d.stage]}.`);
                    this.ctx.election$.next(d);
                    // Rebuild the open tab so it reflects the new stage, then go to the stage's own tab.
                    this.outletKey++;
                    this.router.navigate(['/elections/manage', d.id, this.defaultTab(d.stage)]);
                },
                error: (err) => {
                    this.busy = false;
                    this.toastr.error(apiError(err, 'Error changing the stage.'));
                }
            });
        });
    }

    deleteElection() {
        const e = this.election!;
        Swal.fire({title: `Delete ${e.name}?`, text: 'The election and its positions are removed.', width: 400, position: 'top',
            icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.delete(e.id).subscribe({
                next: () => {
                    this.toastr.success('Election deleted.');
                    this.router.navigate(['/elections/manage']);
                },
                error: (err) => this.toastr.error(apiError(err, 'Error deleting the election.'))
            });
        });
    }

    edited(d: ElectionDetail) {
        this.showEdit = false;
        this.ctx.election$.next(d);
    }
}
