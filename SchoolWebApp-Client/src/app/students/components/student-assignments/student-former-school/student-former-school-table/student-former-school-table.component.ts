import {StudentFormerSchool} from '@/students/models/student-former-school';
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-student-former-school-table',
    templateUrl: './student-former-school-table.component.html',
    styleUrl: './student-former-school-table.component.scss'
})
export class StudentFormerSchoolTableComponent {
    @Input() tableTitle: string = 'Student former schools list';
    @Input() studentFormerSchools: StudentFormerSchool[] = [];
    @Input() showLoginControls: Boolean = false;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    tableHeaders: string[] = [
        'Ref#',
        'Student Full Name',
        'Former School',
        'Class details',
        'Score',
        'Position',
        'Curriculum',
        'Education level',
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
