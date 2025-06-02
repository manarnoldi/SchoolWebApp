import {EducationLevelSubject} from '@/academics/models/education-level-subject';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

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

    constructor() {}

    ngOnInit(): void {}

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

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
