import {Subject} from '@/academics/models/subject';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-subjects-table',
    templateUrl: './subjects-table.component.html',
    styleUrl: './subjects-table.component.scss'
})
export class SubjectsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Subjects list';
    @Input() subjects: Subject[] = [];
    @Input() showLoginControls: Boolean = false;
    @Input() showCheckBoxes: boolean = false;
    @Input() showAllCols: boolean = true;

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
        'Name',
        'Code',
        'Abbr',
        'Lessons #',
        'Optional',
        'Subject group',
        'Department',
        'H.O.S',
        'Description',
        'Action'
    ];

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };

    checkAllClicked = (evt) => {
        this.subjects.forEach((c) => (c.isSelected = evt.target.checked));
    };
}
