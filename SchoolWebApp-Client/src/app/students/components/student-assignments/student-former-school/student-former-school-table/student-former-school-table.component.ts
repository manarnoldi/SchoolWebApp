import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StudentFormerSchool} from '@/students/models/student-former-school';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-student-former-school-table',
    templateUrl: './student-former-school-table.component.html',
    styleUrl: './student-former-school-table.component.scss'
})
export class StudentFormerSchoolTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student former schools list';
    @Input() studentFormerSchools: StudentFormerSchool[] = [];
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
        'Former School',
        'Class details',
        'Score',
        'Position',
        'Curriculum',
        'Education level',
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
