import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {
    apiError,
    ElectionDetail,
    ElectionOptions,
    ElectionPosition,
    GENDER_RULE_LABELS,
    PositionGenderRule,
    PositionScope,
    SavePosition,
    SCOPE_LABELS
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/**
 * Adds or edits an electable position: who votes for it (grades), who may stand,
 * how it is split into contests, seats, a running mate and any gender rule.
 */
@Component({
    selector: 'app-position-form',
    templateUrl: './position-form.component.html'
})
export class PositionFormComponent implements OnInit {
    @Input() options!: ElectionOptions;
    @Input() election!: ElectionDetail;
    @Input() position: ElectionPosition | null = null;
    @Output() saved = new EventEmitter<ElectionDetail>();
    @Output() closed = new EventEmitter<void>();

    model: SavePosition = {
        name: '',
        runningMateTitle: null,
        scope: PositionScope.Single,
        seats: 1,
        genderRule: PositionGenderRule.None,
        genderId: null,
        votersSameGenderOnly: false,
        rank: 0,
        voterLevelIds: [],
        candidateLevelIds: []
    };
    hasRunningMate = false;
    // Off: any voter grade may stand.
    limitCandidates = false;
    saving = false;

    Scope = PositionScope;
    GenderRule = PositionGenderRule;
    scopes = [PositionScope.Single, PositionScope.PerEducationLevel, PositionScope.PerGrade, PositionScope.PerClass];
    genderRules = [PositionGenderRule.None, PositionGenderRule.SingleGender, PositionGenderRule.SeatsPerGender];
    scopeLabels = SCOPE_LABELS;
    genderRuleLabels = GENDER_RULE_LABELS;
    scopeHints: {[s: number]: string} = {
        [PositionScope.Single]: 'e.g. School President: all the chosen grades vote in one race.',
        [PositionScope.PerEducationLevel]: 'e.g. Senator: a separate race in each school section (Upper primary, Junior School...).',
        [PositionScope.PerGrade]: 'e.g. Grade Prefect: a separate race in each grade.',
        [PositionScope.PerClass]: "e.g. Class Representative: a separate race in each class; only the class's students vote."
    };

    constructor(private svc: ElectionsService, private toastr: ToastrService) {}

    ngOnInit(): void {
        const p = this.position;
        if (!p) return;
        this.model = {
            name: p.name,
            runningMateTitle: p.runningMateTitle,
            scope: p.scope,
            seats: p.seats,
            genderRule: p.genderRule,
            genderId: p.genderId,
            votersSameGenderOnly: p.votersSameGenderOnly,
            rank: p.rank,
            voterLevelIds: [...p.voterLevelIds],
            candidateLevelIds: [...p.candidateLevelIds]
        };
        this.hasRunningMate = !!p.runningMateTitle;
        this.limitCandidates = p.candidateLevelIds.length > 0;
    }

    isVoter(id: number): boolean {
        return this.model.voterLevelIds.includes(id);
    }

    isCandidate(id: number): boolean {
        return this.model.candidateLevelIds.includes(id);
    }

    toggleVoter(id: number) {
        if (this.isVoter(id)) {
            this.model.voterLevelIds = this.model.voterLevelIds.filter((x) => x !== id);
            this.model.candidateLevelIds = this.model.candidateLevelIds.filter((x) => x !== id);
        } else {
            this.model.voterLevelIds = [...this.model.voterLevelIds, id];
        }
    }

    toggleCandidate(id: number) {
        this.model.candidateLevelIds = this.isCandidate(id)
            ? this.model.candidateLevelIds.filter((x) => x !== id)
            : [...this.model.candidateLevelIds, id];
    }

    sectionAllVoters(sectionId: number): boolean {
        const s = this.options.sections.find((x) => x.id === sectionId);
        return !!s && s.grades.every((g) => this.isVoter(g.id));
    }

    toggleSection(sectionId: number) {
        const s = this.options.sections.find((x) => x.id === sectionId);
        if (!s) return;
        const ids = s.grades.map((g) => g.id);
        if (this.sectionAllVoters(sectionId)) {
            this.model.voterLevelIds = this.model.voterLevelIds.filter((x) => !ids.includes(x));
            this.model.candidateLevelIds = this.model.candidateLevelIds.filter((x) => !ids.includes(x));
        } else {
            this.model.voterLevelIds = [...new Set([...this.model.voterLevelIds, ...ids])];
        }
    }

    // The sections that have a voter grade - the candidate grades are picked from these.
    get voterSections() {
        return this.options.sections
            .map((s) => ({...s, grades: s.grades.filter((g) => this.isVoter(g.id))}))
            .filter((s) => s.grades.length > 0);
    }

    get runningMateAllowed(): boolean {
        return this.model.seats === 1 && this.model.genderRule !== PositionGenderRule.SeatsPerGender;
    }

    // A rough count of the races the position will split into; the saved position shows them by name.
    get contestPreview(): string {
        const grades = this.model.voterLevelIds.length;
        if (!grades) return 'Choose the grades that vote.';
        switch (Number(this.model.scope)) {
            case PositionScope.Single:
                return 'One race across the chosen grades.';
            case PositionScope.PerEducationLevel:
                return `${this.voterSections.length} race(s): ${this.voterSections.map((s) => s.name).join(', ')}.`;
            case PositionScope.PerGrade:
                return `${grades} race(s), one per grade.`;
            default:
                return `One race per class of the chosen grades in ${this.election.academicYearName}.`;
        }
    }

    get seatsHint(): string {
        const n = Number(this.model.seats) || 1;
        if (Number(this.model.genderRule) === PositionGenderRule.SeatsPerGender)
            return `${n} winner(s) of each gender per race; voters pick up to ${n} of each.`;
        return n === 1 ? 'One winner per race.' : `${n} winners per race; voters pick up to ${n}.`;
    }

    save() {
        const m = this.model;
        const dto: SavePosition = {
            ...m,
            scope: Number(m.scope),
            seats: Number(m.seats) || 1,
            genderRule: Number(m.genderRule),
            genderId: Number(m.genderRule) === PositionGenderRule.SingleGender ? m.genderId : null,
            votersSameGenderOnly: Number(m.genderRule) === PositionGenderRule.SingleGender && m.votersSameGenderOnly,
            runningMateTitle: this.hasRunningMate && this.runningMateAllowed ? (m.runningMateTitle || '').trim() || null : null,
            candidateLevelIds: this.limitCandidates ? m.candidateLevelIds : []
        };
        if (!dto.name.trim()) {
            this.toastr.warning('Enter the position name.');
            return;
        }
        if (dto.voterLevelIds.length === 0) {
            this.toastr.warning('Choose the grades that vote for this position.');
            return;
        }
        if (this.hasRunningMate && this.runningMateAllowed && !dto.runningMateTitle) {
            this.toastr.warning("Enter the running mate's title, e.g. Deputy President.");
            return;
        }
        if (this.limitCandidates && dto.candidateLevelIds.length === 0) {
            this.toastr.warning('Tick the grades candidates may come from.');
            return;
        }
        this.saving = true;
        const call = this.position
            ? this.svc.updatePosition(this.position.id, dto)
            : this.svc.addPosition(this.election.id, dto);
        call.subscribe({
            next: (d) => {
                this.saving = false;
                this.toastr.success('Position saved.');
                this.saved.emit(d);
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(apiError(err, 'Error saving the position.'));
            }
        });
    }
}
