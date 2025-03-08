import {SchoolClass} from '@/class/models/school-class';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import {StudentAttendance} from '@/students/models/student-attendance';
import { StudentDetails } from '@/students/models/student-details';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-student-attendance-table',
    templateUrl: './student-attendance-table.component.html',
    styleUrl: './student-attendance-table.component.scss'
})
export class StudentAttendanceTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student attendance list';
    @Input() studentAttendances: StudentAttendance[] = [];
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() student: StudentDetails;
    @Input() showLoginControls: Boolean = false;

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;
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
    }

    tableHeaders: string[] = [
        'Student Full Name',
        'Adm no',
        'Class',
        'Stream',
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
