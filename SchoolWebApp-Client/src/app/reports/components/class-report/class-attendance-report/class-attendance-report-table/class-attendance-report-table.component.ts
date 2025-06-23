import { StudentAttendancesReport } from '@/reports/models/student-attendance-report';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-class-attendance-report-table',
  templateUrl: './class-attendance-report-table.component.html',
  styleUrl: './class-attendance-report-table.component.scss'
})
export class ClassAttendanceReportTableComponent {
    @Input() tableTitle: string = 'Student attendance report';
    @Input() studentAttendancesRpt: StudentAttendancesReport[] = [];

    @Output() printItemEvent = new EventEmitter<number>();

    tableHeaders: string[] = [
        'Student No',
        'Student Full Name',
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

    printItem = (staffId: number) => {
        this.printItemEvent.emit(staffId);
    };
}
