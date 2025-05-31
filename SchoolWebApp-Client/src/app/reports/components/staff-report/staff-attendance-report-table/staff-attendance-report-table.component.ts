import {StaffAttendancesReport} from '@/reports/models/staff-attendances-report';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StaffAttendance} from '@/staff/models/staff-attendance';
import {Component, Input, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-staff-attendance-report-table',
    templateUrl: './staff-attendance-report-table.component.html',
    styleUrl: './staff-attendance-report-table.component.scss'
})
export class StaffAttendanceReportTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff attendance report';
    @Input() staffAttendancesRpt: StaffAttendancesReport[] = [];

    tableHeaders: string[] = [
        'Staff No',
        'Staff Full Name',
        ...Array.from({length: 31}, (_, i) => (i + 1).toString()),
        ''
    ];
    days: number[] = Array.from({length: 31}, (_, i) => i + 1);
    page = 1;
    pageSize = 20;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    constructor(private tableSettingsSvc: TableSettingsService) {}

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
        this.tableSettingsSvc.changePageSize(20);
    }

    viewItem = (staffId: number) => {};
}
