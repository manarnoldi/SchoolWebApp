import {AcademicYear} from '@/school/models/academic-year';
import {StaffSubject} from '@/staff/models/staff-subject';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-staff-subject-table',
    templateUrl: './staff-subject-table.component.html',
    styleUrl: './staff-subject-table.component.scss'
})
export class StaffSubjectTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff subject assigments';
    @Input() staffSubjects: StaffSubject[] = [];
    @Input() showLoginControls: Boolean = false;

    @Input() academicYears: AcademicYear[];

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    constructor() {}

    ngOnInit(): void {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    tableHeaders: string[] = [
        'Staff full name',
        'Staff number',
        'Year',
        'Class name',
        'Subject',
        'Action'
    ];

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
