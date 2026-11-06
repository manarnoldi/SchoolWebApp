import {Component, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {PLACEMENTS, SubjectPlacement, SubjectRule, TimetableLookups} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';

/**
 * How subjects are placed: left out, kept to mornings or afternoons, taught in
 * doubles, at most so many a day, not last thing. A rule is for every grade or
 * for one grade (which wins). Subjects with no rule use the defaults.
 */
@Component({
    selector: 'app-tt-subject-rules',
    templateUrl: './subject-rules.component.html',
    styleUrl: '../timetable.scss'
})
export class SubjectRulesComponent implements OnInit {
    @Input() lookups!: TimetableLookups;

    rules: SubjectRule[] = [];
    placements = PLACEMENTS;
    loading = true;
    saving = false;
    dirty = false;
    filter = '';

    newSubjectId: number | null = null;
    newLevelId: number | null = null;

    constructor(private svc: TimetableSetupService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.svc.subjectRules().subscribe({
            next: (r) => {
                this.rules = this.sorted(r);
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.error?.message || 'Error loading subject rules.');
            }
        });
    }

    subjectName(id: number): string {
        return this.lookups.subjects.find((s) => s.id === id)?.name ?? '';
    }

    levelName(id: number | null): string {
        return id == null ? 'All grades' : this.lookups.levels.find((l) => l.id === id)?.name ?? '';
    }

    get shown(): SubjectRule[] {
        let f = this.filter.trim().toLowerCase();
        return f ? this.rules.filter((r) => this.subjectName(r.subjectId).toLowerCase().includes(f)) : this.rules;
    }

    private sorted(r: SubjectRule[]): SubjectRule[] {
        let rank = (id: number | null) => (id == null ? -1 : this.lookups.levels.findIndex((l) => l.id === id));
        return [...r].sort((a, b) => this.subjectName(a.subjectId).localeCompare(this.subjectName(b.subjectId)) || rank(a.learningLevelId) - rank(b.learningLevelId));
    }

    add() {
        if (!this.newSubjectId) {
            this.toastr.warning('Pick a subject.');
            return;
        }
        if (this.rules.some((r) => r.subjectId === this.newSubjectId && r.learningLevelId === this.newLevelId)) {
            this.toastr.warning('That subject already has a rule for that grade.');
            return;
        }
        this.rules = this.sorted([...this.rules, {id: 0, subjectId: this.newSubjectId, learningLevelId: this.newLevelId, excluded: false,
            placement: SubjectPlacement.Any, doubleLessons: 0, maxPerDay: 1, avoidLastLesson: false}]);
        this.newSubjectId = null;
        this.newLevelId = null;
        this.dirty = true;
    }

    remove(r: SubjectRule) {
        this.rules = this.rules.filter((x) => x !== r);
        this.dirty = true;
    }

    save() {
        this.saving = true;
        this.svc.saveSubjectRules(this.rules).subscribe({
            next: () => {
                this.saving = false;
                this.dirty = false;
                this.toastr.success('Subject rules saved.');
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(err.error?.message || 'Error saving subject rules.');
            }
        });
    }
}
