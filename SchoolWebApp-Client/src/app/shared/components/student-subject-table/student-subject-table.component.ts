import {AcademicYear} from '@/school/models/academic-year';
import {StudentSubject} from '@/students/models/student-subject';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

// Mirror of StaffSubjectTableComponent but bound to StudentSubject and walking
// the studentClass.student / studentClass.schoolClass relationships to display
// admission no, student name, year, class and subject columns.
@Component({
    selector: 'app-student-subject-table',
    templateUrl: './student-subject-table.component.html',
    styleUrls: ['./student-subject-table.component.scss']
})
export class StudentSubjectTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student subject assignments';
    @Input() studentSubjects: StudentSubject[] = [];
    @Input() showEditDeleteControls: Boolean = true;
    @Input() academicYears: AcademicYear[];

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    tableHeaders: string[] = [
        'Ref#',
        'Student full name',
        'Adm no',
        'Year',
        'Class name',
        'Subject'
    ];

    constructor() {}

    ngOnInit(): void {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };

    viewItem = (id: number) => {
        this.viewItemEvent.emit(id);
    };
}
