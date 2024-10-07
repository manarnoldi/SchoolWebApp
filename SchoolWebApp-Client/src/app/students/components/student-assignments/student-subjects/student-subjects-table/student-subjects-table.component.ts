import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StudentDetails} from '@/students/models/student-details';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, Subscription} from 'rxjs';

@Component({
    selector: 'app-student-subjects-table',
    templateUrl: './student-subjects-table.component.html',
    styleUrl: './student-subjects-table.component.scss'
})
export class StudentSubjectsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Student subjects list';
    @Input() studentSubjects: StudentSubject[] = [];

    @Output() viewItemEvent = new EventEmitter<number>();
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    students: StudentDetails[] = [];
    schoolClasses: SchoolClass[] = [];

    constructor(
        private tableSettingsSvc: TableSettingsService,
        private studentsSvc: StudentDetailsService,
        private schoolClassSvc: SchoolClassesService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );

        let studentsReq = this.studentsSvc.get('/students');
        let schoolClassReq = this.schoolClassSvc.get('/schoolClasses');

        forkJoin([studentsReq, schoolClassReq]).subscribe(
            ([students, schoolClasses]) => {
                this.students = students;
                this.schoolClasses = schoolClasses;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    tableHeaders: string[] = [
        'Student Full Name',
        'Class',
        'Stream',
        'Year',
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

    viewItem = (id: number) => {
        this.viewItemEvent.emit(id);
    };
}
