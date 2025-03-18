import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-school-class-minimum-table',
    templateUrl: './school-class-minimum-table.component.html',
    styleUrl: './school-class-minimum-table.component.scss'
})
export class SchoolClassMinimumTableComponent implements OnInit {
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() minimumTable: Boolean = false;
    @ViewChild('checkAll', {static: false}) checkAll: ElementRef;

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

    checkAllClicked = (inputSelectAll: any) => {
        if (inputSelectAll?.target?.checked) {
            this.schoolClasses.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.schoolClasses.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.schoolClasses.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };
}
