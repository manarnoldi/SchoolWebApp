import {StaffAttendancesReport} from '@/reports/models/staff-attendances-report';
import {Component, Input, OnInit} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-staff-attendance-report-table',
    templateUrl: './staff-attendance-report-table.component.html',
    styleUrl: './staff-attendance-report-table.component.scss'
})
export class StaffAttendanceReportTableComponent {
    @Input() tableTitle: string = 'Staff attendance report';
    @Input() staffAttendancesRpt: StaffAttendancesReport[] = [];

    tableHeaders: string[] = [
        'Staff No',
        'Staff Full Name',
        ...Array.from({length: 31}, (_, i) => (i + 1).toString()),
        ''
    ];

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    days: number[] = Array.from({length: 31}, (_, i) => i + 1);
    page = 1;
    pageSize = 20;
    viewItem = (staffId: number) => {};
}
