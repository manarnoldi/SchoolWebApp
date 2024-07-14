import { ClassLeadership } from '@/class/models/class-leadership';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import {Component, EventEmitter, Input, Output} from '@angular/core';
import { Subscription } from 'rxjs';

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
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;
    
    constructor(private tableSettingsSvc: TableSettingsService) { }
    
    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    viewItem = (id: number) => {
        this.viewItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };
}
