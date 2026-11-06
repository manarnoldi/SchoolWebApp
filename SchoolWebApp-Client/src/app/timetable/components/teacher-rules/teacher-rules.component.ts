import {Component, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {TeacherRule, TimetableLookups, WEEK_DAYS} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';

/**
 * Teachers' limits (lessons a day, lessons in a row) and times they cannot
 * teach. Times are clock times, so they hold for every bell schedule.
 */
@Component({
    selector: 'app-tt-teacher-rules',
    templateUrl: './teacher-rules.component.html',
    styleUrl: '../timetable.scss'
})
export class TeacherRulesComponent implements OnInit {
    @Input() lookups!: TimetableLookups;

    rules: TeacherRule[] = [];
    loading = true;
    days = WEEK_DAYS;
    newStaffId: number | null = null;
    // Teachers with unsaved changes.
    dirty = new Set<number>();
    saving = new Set<number>();

    constructor(private svc: TimetableSetupService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.svc.teacherRules().subscribe({
            next: (r) => {
                this.rules = r;
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.error?.message || 'Error loading teacher limits.');
            }
        });
    }

    get available() {
        return this.lookups.teachers.filter((t) => !this.rules.some((r) => r.staffDetailsId === t.id));
    }

    add() {
        let t = this.lookups.teachers.find((x) => x.id === this.newStaffId);
        if (!t) return;
        this.rules = [{staffDetailsId: t.id, staffName: t.name, maxLessonsPerDay: null, maxConsecutiveLessons: null, notes: null,
            unavailable: []}, ...this.rules];
        this.dirty.add(t.id);
        this.newStaffId = null;
    }

    addTime(r: TeacherRule) {
        r.unavailable.push({id: 0, day: 1, startTime: '08:00', endTime: '10:00', reason: null});
        this.dirty.add(r.staffDetailsId);
    }

    removeTime(r: TeacherRule, i: number) {
        r.unavailable.splice(i, 1);
        this.dirty.add(r.staffDetailsId);
    }

    touch(r: TeacherRule) {
        this.dirty.add(r.staffDetailsId);
    }

    save(r: TeacherRule) {
        this.saving.add(r.staffDetailsId);
        this.svc.saveTeacherRule(r).subscribe({
            next: () => {
                this.saving.delete(r.staffDetailsId);
                this.dirty.delete(r.staffDetailsId);
                this.toastr.success(`Saved for ${r.staffName}.`);
            },
            error: (err) => {
                this.saving.delete(r.staffDetailsId);
                this.toastr.error(err.error?.message || 'Error saving.');
            }
        });
    }

    clear(r: TeacherRule) {
        r.maxLessonsPerDay = null;
        r.maxConsecutiveLessons = null;
        r.notes = null;
        r.unavailable = [];
        this.save(r);
        this.rules = this.rules.filter((x) => x !== r);
    }
}
