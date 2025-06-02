import {StudentDiscipline} from '@/students/models/student-discipline';
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-student-discipline-table',
    templateUrl: './student-discipline-table.component.html',
    styleUrl: './student-discipline-table.component.scss'
})
export class StudentDisciplineTableComponent {
    @Input() tableTitle: string = 'Student discipline records';
    @Input() studentDisciplines: StudentDiscipline[] = [];
    @Input() showLoginControls: Boolean = false;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    tableHeaders: string[] = [
        'Student Full Name',
        'From Date',
        'To Date',
        'Occurence Details',
        'Occurence Type',
        'Outcome',
        'Action'
    ];

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
