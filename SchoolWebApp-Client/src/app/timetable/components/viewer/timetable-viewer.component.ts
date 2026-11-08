import {Component, Input, OnChanges, OnInit} from '@angular/core';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {TimetableDetail} from '@/timetable/models/timetable-models';
import {dayName, GridEntry, MasterGrid, TimetableIndex, WeekGrid} from '@/timetable/utils/timetable-grid';
import {PrintSchool, printClasses, printMaster, printTeachers} from '@/timetable/utils/timetable-print';

type ViewMode = 'class' | 'teacher' | 'master';

/**
 * A timetable shown three ways: one class's week, one teacher's week, or the
 * master timetable (every class, days across). Prints what it shows. Used by
 * the timetable page, the published timetables and My Timetable.
 */
@Component({
    selector: 'app-tt-viewer',
    templateUrl: './timetable-viewer.component.html',
    styleUrl: '../timetable.scss'
})
export class TimetableViewerComponent implements OnInit, OnChanges {
    @Input() timetable!: TimetableDetail;
    // My Timetable: just this teacher.
    @Input() onlyTeacherId: number | null = null;

    idx!: TimetableIndex;
    mode: ViewMode = 'class';
    classId: number | null = null;
    staffId: number | null = null;
    classGrid: WeekGrid | null = null;
    teacherGrid: WeekGrid | null = null;
    masters: MasterGrid[] = [];
    school: PrintSchool = {name: ''};
    dayName = dayName;

    constructor(private schoolSvc: SchoolDetailsService) {}

    ngOnInit(): void {
        this.schoolSvc.get('/schoolDetails').subscribe({next: (r: any) => {
            let s = (r || [])[0] || r;
            this.school = {name: s?.name || '', address: s?.address, logo: s?.logoAsBase64};
        }});
    }

    ngOnChanges(): void {
        if (!this.timetable) return;
        this.idx = new TimetableIndex(this.timetable);
        if (this.onlyTeacherId) {
            this.mode = 'teacher';
            this.staffId = this.onlyTeacherId;
        }
        if (!this.classId || !this.timetable.classes.some((c) => c.id === this.classId)) this.classId = this.timetable.classes[0]?.id ?? null;
        if (!this.staffId || !this.timetable.teachers.some((p) => p.id === this.staffId)) this.staffId = this.onlyTeacherId ?? this.timetable.teachers[0]?.id ?? null;
        this.refresh();
    }

    refresh() {
        this.classGrid = this.classId ? this.idx.classWeek(this.classId) : null;
        this.teacherGrid = this.staffId ? this.idx.teacherWeek(this.staffId) : null;
        this.masters = this.mode === 'master'
            ? this.timetable.schedules.filter((s) => this.timetable.classes.some((c) => c.scheduleId === s.id)).map((s) => this.idx.master(s))
            : [];
    }

    setMode(m: ViewMode) {
        this.mode = m;
        this.refresh();
    }

    /** Opens a class or a teacher from the master timetable. */
    openClass(id: number) {
        this.classId = id;
        this.setMode('class');
    }

    openTeacher(id: number | null) {
        if (!id || this.onlyTeacherId) return;
        this.staffId = id;
        this.setMode('teacher');
    }

    get teacherLessons(): number {
        return this.timetable.cards.filter((c) => c.staffDetailsId === this.staffId && c.day).reduce((n, c) => n + c.length, 0);
    }

    subjectColour(e: GridEntry): string {
        // A steady pastel per subject, so a subject is easy to follow across the week.
        let h = (e.card.subjectId * 47) % 360;
        return `hsl(${h}, 70%, 92%)`;
    }

    // ------------------------------------------------------------ printing

    printClass() {
        if (this.classId) printClasses(this.timetable, [this.classId], this.school);
    }

    printAllClasses() {
        printClasses(this.timetable, this.timetable.classes.map((c) => c.id), this.school);
    }

    printTeacher() {
        if (this.staffId) printTeachers(this.timetable, [this.staffId], this.school);
    }

    printAllTeachers() {
        printTeachers(this.timetable, this.timetable.teachers.map((p) => p.id), this.school);
    }

    printMaster() {
        printMaster(this.timetable, this.school);
    }
}
