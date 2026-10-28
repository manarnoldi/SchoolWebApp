import {Component, OnDestroy, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {concat, Subscription, toArray} from 'rxjs';
import Swal from 'sweetalert2';
import {
    apiError,
    ElectionDetail,
    ElectionPosition,
    PositionGenderRule,
    SCOPE_LABELS
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';
import {ElectionContext} from '@/elections/services/election-context.service';

/** Election page, Positions tab: the electable positions and the races they split into. */
@Component({
    selector: 'app-election-positions',
    templateUrl: './election-positions.component.html'
})
export class ElectionPositionsComponent implements OnInit, OnDestroy {
    election: ElectionDetail | null = null;
    busy = false;
    editPosition: ElectionPosition | null = null;
    showPositionForm = false;
    expanded = new Set<number>();
    scopeLabels = SCOPE_LABELS;

    private sub?: Subscription;

    constructor(private svc: ElectionsService, private toastr: ToastrService, public ctx: ElectionContext) {}

    ngOnInit(): void {
        this.sub = this.ctx.election$.subscribe((e) => this.election = e);
    }

    ngOnDestroy(): void {
        this.sub?.unsubscribe();
    }

    get canEditPositions(): boolean {
        return this.ctx.canManage && !!this.election?.positionsEditable;
    }

    // "Grade 4 - Grade 9", or the section names when whole sections vote.
    describeGrades(ids: number[]): string {
        const options = this.ctx.options;
        if (!options || ids.length === 0) return '';
        const parts: string[] = [];
        for (const s of options.sections) {
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
        this.ctx.election$.next(d);
    }

    deletePosition(p: ElectionPosition) {
        Swal.fire({title: `Remove ${p.name}?`, text: 'Its races are removed with it.', width: 400, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Remove', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.deletePosition(p.id).subscribe({
                next: (d) => this.ctx.election$.next(d),
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
                if (done.length) this.ctx.election$.next(done[done.length - 1]);
            },
            error: (err) => {
                this.busy = false;
                this.toastr.error(apiError(err, "Error reordering the positions."));
                this.ctx.reload();
            }
        });
    }
}
