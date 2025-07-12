import {StaffAttendance} from '@/staff/models/staff-attendance';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
@Component({
    selector: 'app-staff-attendance-table',
    templateUrl: './staff-attendance-table.component.html',
    styleUrl: './staff-attendance-table.component.scss'
})
export class StaffAttendanceTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff attendance list';
    @Input() staffAttendances: StaffAttendance[] = [];
    @Input() showLoginControls: Boolean = false;
    @Input() showEditDeleteControls: Boolean = true;

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    constructor() {}

    ngOnInit(): void {}

    tableHeaders: string[] = [
        'Ref#',
        'Staff No',
        'Staff Full Name',
        'Month',
        'Year',
        'Date',
        'Present?',
        'Remarks'
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
}
