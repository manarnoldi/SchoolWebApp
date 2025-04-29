import { TableSettingsService } from '@/shared/services/table-settings.service';
import { StudentSubject } from '@/students/models/student-subject';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-students-subjects-table',
    templateUrl: './students-subjects-table.component.html',
    styleUrl: './students-subjects-table.component.scss'
})
export class StudentsSubjectsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student subjects list';
    @Input() studentSubjects: StudentSubject[] = [];

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
        'Adm No',
        'Student Full Name',
        'Subject Code',
        'Subject name',
        'Action'
    ];

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };
}
