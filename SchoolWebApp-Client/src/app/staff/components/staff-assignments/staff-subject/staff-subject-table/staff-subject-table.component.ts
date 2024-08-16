import { LearningLevel } from '@/class/models/learning-level';
import { SchoolStream } from '@/class/models/school-stream';
import { AcademicYear } from '@/school/models/academic-year';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import { StaffSubject } from '@/staff/models/staff-subject';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-staff-subject-table',
    templateUrl: './staff-subject-table.component.html',
    styleUrl: './staff-subject-table.component.scss'
})
export class StaffSubjectTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff subject assigments';
    @Input() staffSubjects: StaffSubject[] = [];
    @Input() showLoginControls: Boolean = false;

    @Input() academicYears: AcademicYear[];
    @Input() learningLevels: LearningLevel[];
    @Input() schoolStreams: SchoolStream[];

    @Output() viewItemEvent = new EventEmitter<number>();
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
        'Staff full name',
        'Staff number',
        'Year',
        'Class',
        'Stream',
        'Subject',
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
