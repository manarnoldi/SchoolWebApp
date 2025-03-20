import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {AcademicYear} from '@/school/models/academic-year';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-exam-table',
    templateUrl: './exam-table.component.html',
    styleUrl: './exam-table.component.scss'
})
export class ExamTableComponent implements OnInit {
    @Input() tableTitle: string = 'Examinations list';
    @Input() eduLevelId: number;
    @Input() exams: Exam[] = [];
    @Input() subjects: Subject[] = [];
    @Input() examTypes: ExamType[] = [];
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() sessions: Session[] = [];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];

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
        'Curriculum',
        'Year',
        'Session',
        'Class',
        'Exam Type',
        'Subject',
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
}
