import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {concat, toArray} from 'rxjs';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {
    apiError,
    ElectionDetail,
    ElectionOptions,
    ElectionPosition,
    ElectionStage,
    GENDER_RULE_LABELS,
    PositionGenderRule,
    SCOPE_LABELS,
    STAGE_ACTIONS,
    STAGE_BADGES,
    STAGE_LABELS,
    VoterRegisterMode
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

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

/** One election: its details, electable positions and the races they split into, and its stage. */
@Component({
    selector: 'app-election-detail',
    templateUrl: './election-detail.component.html'
})
export class ElectionDetailComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'},
        {link: [], title: 'Election'}
    ];
    dashboardTitle = 'Elections: Election';

    options: ElectionOptions | null = null;
    election: ElectionDetail | null = null;
    loading = false;
    busy = false;
    showEdit = false;
    editPosition: ElectionPosition | null = null;
    showPositionForm = false;
    expanded = new Set<number>();

    steps = [ElectionStage.Draft, ElectionStage.Nominations, ElectionStage.CandidatesFinal,
        ElectionStage.Polling, ElectionStage.PollingClosed, ElectionStage.Declared];
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    stageActions = STAGE_ACTIONS;
    scopeLabels = SCOPE_LABELS;
    genderRuleLabels = GENDER_RULE_LABELS;
    Mode = VoterRegisterMode;
    GenderRule = PositionGenderRule;
    Stage = ElectionStage;

    private gradeNames = new Map<number, string>();

    constructor(private svc: ElectionsService, private toastr: ToastrService,
                private route: ActivatedRoute, public router: Router) {}

    ngOnInit(): void {
        this.svc.options().subscribe({
            next: (o) => {
                this.options = o;
                o.sections.forEach((s) => s.grades.forEach((g) => this.gradeNames.set(g.id, g.name)));
            },
            error: (err) => this.toastr.error(apiError(err, 'Error loading the election options.'))
        });
        this.route.paramMap.subscribe((p) => this.load(Number(p.get('id'))));
    }

    load(id: number) {
        this.loading = true;
        this.svc.get(id).subscribe({
            next: (d) => {
                this.loading = false;
                this.show(d);
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
        this.breadcrumbs = [...this.breadcrumbs.slice(0, 2), {link: ['/elections/manage', String(d.id)], title: d.name}];
        this.dashboardTitle = 'Elections: ' + d.name;
    }

    get canManage(): boolean {
        return !!this.options?.canManage;
    }

    get canEditPositions(): boolean {
        return this.canManage && !!this.election?.positionsEditable;
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

    // "Grade 4 - Grade 9", or the section names when whole sections vote.
    describeGrades(ids: number[]): string {
        if (!this.options || ids.length === 0) return '';
        const parts: string[] = [];
        for (const s of this.options.sections) {
            const picked = s.grades.filter((g) => ids.includes(g.id));
            if (picked.length === 0) continue;
            if (picked.length === s.grades.length && s.grades.length > 1) parts.push(s.name);
            else parts.push(...picked.map((g) => g.name));
        }
        return parts.join(', ');
    }

    genderText(p: ElectionPosition): string {
        if (p.genderRule === PositionGenderRule.SingleGender)
            return `${p.genderName || ''} candidates` + (p.votersSameGenderOnly ? `, ${p.genderName || ''} voters` : '');
        if (p.genderRule === PositionGenderRule.SeatsPerGender) return `${p.seats} per gender`;
        return '';
    }

    toggleRaces(p: ElectionPosition) {
        if (this.expanded.has(p.id)) this.expanded.delete(p.id);
        else this.expanded.add(p.id);
    }

    addPosition() {
        this.editPosition = null;
        this.showPositionForm = true;
    }

    openPosition(p: ElectionPosition) {
        this.editPosition = p;
        this.showPositionForm = true;
    }

    positionSaved(d: ElectionDetail) {
        this.showPositionForm = false;
        this.show(d);
    }

    deletePosition(p: ElectionPosition) {
        Swal.fire({title: `Remove ${p.name}?`, text: 'Its races are removed with it.', width: 400, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Remove', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.deletePosition(p.id).subscribe({
                next: (d) => this.show(d),
                error: (err) => this.toastr.error(apiError(err, 'Error removing the position.'))
            });
        });
    }

    // Moves up or down one place, renumbering the list 1..n (deletes leave gaps).
    move(p: ElectionPosition, by: number) {
        const list = [...this.election!.positionList];
        const i = list.indexOf(p), j = i + by;
        if (j < 0 || j >= list.length) return;
        [list[i], list[j]] = [list[j], list[i]];
        const saves = list
            .map((x, k) => ({x, rank: k + 1}))
            .filter(({x, rank}) => x.rank !== rank)
            .map(({x, rank}) => this.svc.updatePosition(x.id, {
                name: x.name, runningMateTitle: x.runningMateTitle, scope: x.scope, seats: x.seats, genderRule: x.genderRule,
                genderId: x.genderId, votersSameGenderOnly: x.votersSameGenderOnly, rank,
                voterLevelIds: x.voterLevelIds, candidateLevelIds: x.candidateLevelIds
            }));
        this.busy = true;
        concat(...saves).pipe(toArray()).subscribe({
            next: (done) => {
                this.busy = false;
                if (done.length) this.show(done[done.length - 1]);
            },
            error: (err) => {
                this.busy = false;
                this.toastr.error(apiError(err, "Error reordering the positions."));
                this.load(this.election!.id);
            }
        });
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
                    this.show(d);
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
        this.show(d);
    }
}
