import {Component, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {OptionGroup, TimetableLookups} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';

/**
 * Subjects a class takes at the same time, learners splitting between them -
 * CRE / IRE / HRE, or an elective block. The group meets as often as its
 * busiest subject, each subject with its own teacher.
 */
@Component({
    selector: 'app-tt-option-groups',
    templateUrl: './option-groups.component.html',
    styleUrl: '../timetable.scss'
})
export class OptionGroupsComponent implements OnInit {
    @Input() lookups!: TimetableLookups;

    groups: OptionGroup[] = [];
    loading = true;
    saving = false;
    dirty = false;
    // Subject search per group (by index).
    search: {[i: number]: string} = {};

    constructor(private svc: TimetableSetupService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.svc.optionGroups().subscribe({
            next: (g) => {
                this.groups = g;
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.error?.message || 'Error loading option groups.');
            }
        });
    }

    add(religion = false) {
        let ids = religion
            ? this.lookups.subjects.filter((s) => /religious education/i.test(s.name)).map((s) => s.id)
            : [];
        this.groups = [...this.groups, {id: 0, name: religion ? 'Religious Education' : '', learningLevelId: null, subjectIds: ids}];
        this.dirty = true;
    }

    remove(g: OptionGroup) {
        this.groups = this.groups.filter((x) => x !== g);
        this.dirty = true;
    }

    toggle(g: OptionGroup, subjectId: number) {
        g.subjectIds = g.subjectIds.includes(subjectId) ? g.subjectIds.filter((x) => x !== subjectId) : [...g.subjectIds, subjectId];
        this.dirty = true;
    }

    subjectsFor(i: number, g: OptionGroup) {
        // The chosen subjects, plus those matching the search (all of them would be a wall of chips).
        let f = (this.search[i] || '').trim().toLowerCase();
        return this.lookups.subjects.filter((s) => g.subjectIds.includes(s.id) || (f && (s.name.toLowerCase().includes(f) || s.abbr.toLowerCase().includes(f))));
    }

    get hasReligionGroup(): boolean {
        return this.groups.some((g) => /relig/i.test(g.name));
    }

    save() {
        this.saving = true;
        this.svc.saveOptionGroups(this.groups).subscribe({
            next: () => {
                this.saving = false;
                this.dirty = false;
                this.toastr.success('Option groups saved.');
                this.ngOnInit();
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(err.error?.message || 'Error saving option groups.');
            }
        });
    }
}
