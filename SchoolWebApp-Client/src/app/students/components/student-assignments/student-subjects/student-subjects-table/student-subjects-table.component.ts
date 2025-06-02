import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
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

    students: StudentDetails[] = [];
    schoolClasses: SchoolClass[] = [];

    constructor(
        private studentsSvc: StudentDetailsService,
        private schoolClassSvc: SchoolClassesService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
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

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

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
