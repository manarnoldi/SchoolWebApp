import {StudentSubject} from '@/students/models/student-subject';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-students-subjects-table',
    templateUrl: './students-subjects-table.component.html',
    styleUrl: './students-subjects-table.component.scss'
})
export class StudentsSubjectsTableComponent {
    @Input() tableTitle: string = 'Student subjects list';
    @Input() studentSubjects: StudentSubject[] = [];

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    tableHeaders: string[] = [
        'Adm No',
        'Student Full Name',
        'Subject Code',
        'Subject name',
        'Action'
    ];

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };
}
