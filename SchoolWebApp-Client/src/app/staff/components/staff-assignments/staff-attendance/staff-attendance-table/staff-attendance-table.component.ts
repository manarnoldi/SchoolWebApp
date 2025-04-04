import { TableSettingsService } from '@/shared/services/table-settings.service';
import {StaffAttendance} from '@/staff/models/staff-attendance';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-staff-attendance-table',
    templateUrl: './staff-attendance-table.component.html',
    styleUrl: './staff-attendance-table.component.scss'
})
export class StaffAttendanceTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff attendance list';
    @Input() staffAttendances: StaffAttendance[] = [];
    @Input() showLoginControls: Boolean = false;

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    constructor(private tableSettingsSvc: TableSettingsService,) {
        
    }

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

    tableHeaders: string[] = [
        'Staff Full Name',
        'Month',
        'Year',
        'Date',
        'Present?',
        'Remarks',
        'Action'
    ];

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };
}
