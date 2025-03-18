import {EducationLevelSubject} from '@/academics/models/education-level-subject';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-education-level-subjects-table',
    templateUrl: './education-level-subjects-table.component.html',
    styleUrl: './education-level-subjects-table.component.scss'
})
export class EducationLevelSubjectsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Education level subjects list';
    @Input() educationLevelSubjects: EducationLevelSubject[] = [];

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
        'Academic Year',
        'Education Level',
        'Subject',
        'Description',
        'Action'
    ];

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    editItem = (id: number) => {
        this.editItemEvent.emit(id);
    };
}
