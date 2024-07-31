import { TableSettingsService } from '@/shared/services/table-settings.service';
import { StudentDiscipline } from '@/students/models/student-discipline';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-student-discipline-table',
    templateUrl: './student-discipline-table.component.html',
    styleUrl: './student-discipline-table.component.scss'
})
export class StudentDisciplineTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student discipline records';
    @Input() studentDisciplines: StudentDiscipline[] = [];
    @Input() showLoginControls: Boolean = false;

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
        'Student Full Name',
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
