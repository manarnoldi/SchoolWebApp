import {AcademicYear} from '@/school/models/academic-year';
import {SchoolEvent} from '@/school/models/schoolEvent';
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-events-table',
    templateUrl: './events-table.component.html',
    styleUrl: './events-table.component.scss'
})
export class EventsTableComponent {
    @Input() events: SchoolEvent[] = [];
    @Input() academicYears: AcademicYear[] = [];

    @Output() deleteItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() resetFormEvent = new EventEmitter<void>();

    page = 1;
    pageSize = 10;

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

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    resetForm = () => {
        this.resetFormEvent.emit();
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
