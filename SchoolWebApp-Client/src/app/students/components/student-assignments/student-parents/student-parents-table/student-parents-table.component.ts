import {Status} from '@/core/enums/status';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StudentParent} from '@/students/models/student-parent';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-student-parents-table',
    templateUrl: './student-parents-table.component.html',
    styleUrl: './student-parents-table.component.scss'
})
export class StudentParentsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student parents list';
    @Input() studentParents: StudentParent[] = [];
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

    statusVals = Status;
    statusValues;

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );

        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );
    }

    tableHeaders: string[] = [
        'Full name',
        'Occupation',
        'Nationality',
        'Notifiable',
        'Payer',
        'Picker',
        'Status',
        'Relationship',
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
