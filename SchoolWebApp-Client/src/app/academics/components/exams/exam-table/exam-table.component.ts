import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {AcademicYear} from '@/school/models/academic-year';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-exam-table',
    templateUrl: './exam-table.component.html',
    styleUrl: './exam-table.component.scss'
})
export class ExamTableComponent implements OnInit {
    @Input() tableTitle: string = 'Examinations list';
    @Input() eduLevelId: number;
    @Input() exams: Exam[] = [];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    constructor() {}

    ngOnInit(): void {}

    tableHeaders: string[] = [
        'Ref#',
        'Curriculum',
        'Year',
        'Session',
        'Class',
        'Subject',
        'Exam Type',        
        'Exam Name',
        'Exam Mark',
        'Contributing',
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
