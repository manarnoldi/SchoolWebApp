import {ClassLeadership} from '@/class/models/class-leadership';
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-class-leadership-table',
    templateUrl: './class-leadership-table.component.html',
    styleUrl: './class-leadership-table.component.scss'
})
export class ClassLeadershipTableComponent {
    @Input() tableTitle: string = 'Class leadership roles list';
    @Input() classLeaderships: ClassLeadership[] = [];
    @Input() personTypes;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    tableHeaders: string[] = [
        'Role for',
        'Role name',
        'Full name',
        'Description',
        'Action'
    ];

    page = 1;
    pageSize = 10;

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    viewItem = (id: number) => {
        this.viewItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
