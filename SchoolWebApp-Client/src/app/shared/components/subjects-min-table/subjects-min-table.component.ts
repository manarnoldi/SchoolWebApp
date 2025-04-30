import {Subject} from '@/academics/models/subject';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-subjects-min-table',
    templateUrl: './subjects-min-table.component.html',
    styleUrl: './subjects-min-table.component.scss'
})
export class SubjectsMinTableComponent implements OnInit {
    @Input() subjects: Subject[] = [];
    @Input() minimumTable: Boolean = false;
    @Input() disabled: Boolean = false;
    @Input() tableTitle = "Subjects Mini Table";
    
    @ViewChild("checkAllSubjects", {static: false}) checkAll: ElementRef;

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
            this.subjects.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.subjects.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.subjects.some(s => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    }
}
