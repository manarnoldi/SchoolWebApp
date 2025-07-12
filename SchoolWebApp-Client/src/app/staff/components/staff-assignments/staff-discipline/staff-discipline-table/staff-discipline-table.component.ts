import {StaffDiscipline} from '@/staff/models/staff-discipline';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-staff-discipline-table',
    templateUrl: './staff-discipline-table.component.html',
    styleUrl: './staff-discipline-table.component.scss'
})
export class StaffDisciplineTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff discipline records';
    @Input() staffDisciplines: StaffDiscipline[] = [];
    @Input() showLoginControls: Boolean = false;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    constructor() {}

    pageChanged = (page: number) => {
        this.page = page;
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    ngOnInit(): void {}

    tableHeaders: string[] = [
        'Ref#',
        'Staff Full Name',
        'From Date',
        'To Date',
        'Occurence Details',
        'Occurence Type',
        'Outcome',
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
