import {Subject} from '@/academics/models/subject';
import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';

@Component({
    selector: 'app-subjects-min-table',
    templateUrl: './subjects-min-table.component.html',
    styleUrl: './subjects-min-table.component.scss'
})
export class SubjectsMinTableComponent implements OnInit {
    @Input() subjects: Subject[] = [];
    @Input() minimumTable: Boolean = false;
    @Input() disabled: Boolean = false;
    @Input() tableTitle = 'Subjects Mini Table';

    @ViewChild('checkAllSubjects', {static: false}) checkAll: ElementRef;

    page = 1;
    pageSize = 10;

    constructor() {}

    ngOnInit(): void {}

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

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.subjects.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };
}
