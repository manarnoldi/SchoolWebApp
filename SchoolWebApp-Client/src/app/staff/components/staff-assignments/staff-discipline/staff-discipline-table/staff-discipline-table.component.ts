import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StaffDiscipline} from '@/staff/models/staff-discipline';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-staff-discipline-table',
    templateUrl: './staff-discipline-table.component.html',
    styleUrl: './staff-discipline-table.component.scss'
})
export class StaffDisciplineTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff discipline records';
    @Input() staffDisciplines: StaffDiscipline[] = [];
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
        'Staff Full Name',
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
