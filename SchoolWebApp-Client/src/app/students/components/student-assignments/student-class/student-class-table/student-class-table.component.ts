import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {AcademicYear} from '@/school/models/academic-year';
import {StudentClass} from '@/students/models/student-class';
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-student-class-table',
    templateUrl: './student-class-table.component.html',
    styleUrl: './student-class-table.component.scss'
})
export class StudentClassTableComponent {
    @Input() tableTitle: string = 'Student classes list';
    @Input() studentClasses: StudentClass[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() schoolStreams: SchoolStream[] = [];
    @Input() learningLevels: LearningLevel[] = [];
    @Input() showLoginControls: Boolean = false;

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;

    tableHeaders: string[] = [
        'Ref#',
        'Student Full Name',
        'Academic Year',
        'Class',
        'Stream',
        'Description',
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

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
