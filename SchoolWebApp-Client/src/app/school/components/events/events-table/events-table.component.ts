import {AcademicYear} from '@/school/models/academic-year';
import {SchoolEvent} from '@/school/models/schoolEvent';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-events-table',
    templateUrl: './events-table.component.html',
    styleUrl: './events-table.component.scss'
})
export class EventsTableComponent implements OnInit {
    @Input() events: SchoolEvent[] = [];
    @Input() academicYears: AcademicYear[] = [];

    @Output() deleteItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() resetFormEvent = new EventEmitter<void>();

    page = 1;
    pageSize = 10;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableTitle: string = ' Events list';
    tableHeaders: string[] = [
        'Academic year',
        'Session',
        'Event name',
        'Event location',
        'Start date',
        'End date',
        'Description',
        'Action'
    ];

    tableModel: string = 'event';

    constructor(private tableSettingsSvc: TableSettingsService) {}

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    resetForm = () => {
        this.resetFormEvent.emit();
    };
}
