import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {StudentDetails} from '@/students/models/student-details';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {AfterViewInit, Component, Input, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {StudentClassFormComponent} from './student-class-form/student-class-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentClass} from '@/students/models/student-class';
import {StudentClassService} from '@/students/services/student-class.service';
import Swal from 'sweetalert2';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-student-class',
    templateUrl: './student-class.component.html',
    styleUrl: './student-class.component.scss'
})
export class StudentClassComponent implements AfterViewInit {
    @Input() statuses;
    @Input() student: StudentDetails;
    @Input() academicYears: AcademicYear[];
    @Input() schoolStreams: SchoolStream[];
    @Input() learningLevels: LearningLevel[];
    @ViewChild(StudentClassFormComponent)
    studentClassFormComponent: StudentClassFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    firstLoad: boolean = true;

    studentId: number = 0;
    studentClass: StudentClass;
    studentClasses: StudentClass[];

    schoolClasses: SchoolClass[];

    constructor(
        private toastr: ToastrService,
        private studentClassSvc: StudentClassService,
        private route: ActivatedRoute,
        private schoolClassesSvc: SchoolClassesService
    ) {}

    ngAfterViewInit(): void {
        this.loadStudentClasses();
    }

    loadStudentClasses = () => {
        const year = this.academicYears.sort((a, b) => b.rank - a.rank)[0];
        let dmy = new SchoolSoftFilter();
        dmy.academicYearId = parseInt(year.id);
        this.ssFilterFormComponent.setFormControls(dmy);
        this.searchForClasses(dmy);
    };

    yearChanged = (yearId: number) => {
        this.schoolClasses = [];
        if (!yearId || yearId <= 0) {
            this.toastr.error('Select year first!');
            return;
        }
        this.schoolClassesSvc.getByAcademicYearId(yearId).subscribe({
            next: (schoolClasses) => {
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    academicYearChanged = (id: number) => {
        this.studentClasses = [];
    };

    searchForClasses = (cyf: SchoolSoftFilter) => {
        if (!cyf.academicYearId) {
            if (!this.firstLoad) {
                this.toastr.error(
                    'Select academic year before clicking search!'
                );
            }
            return;
        }
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            this.studentClassSvc
                .getByStudentYearId(this.studentId, cyf.academicYearId)
                .subscribe({
                    next: (studentClasses) => {
                        this.studentClasses = studentClasses;
                        if (
                            this.studentClasses.length <= 0 &&
                            !this.firstLoad
                        ) {
                            this.toastr.info(
                                'No student classes record/s found for the search parameters!'
                            );
                        }
                        this.firstLoad = false;
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        });
    };

    editItem(id: number, action = 'edit') {
        this.studentClassSvc.getById(id, '/studentClasses').subscribe(
            (res) => {
                let studentClassId = res.id;
                this.studentClass = new StudentClass(res);
                this.studentClass.id = studentClassId;
                this.studentClassFormComponent.setFormControls(
                    this.studentClass
                );
                this.studentClassFormComponent.action = action;
                this.studentClassFormComponent.studentClass = this.studentClass;
                this.tableButton.onClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.studentClassSvc.delete('/studentClasses', id).subscribe(
                    (res) => {
                        this.loadStudentClasses();
                        this.toastr.success('Record deleted successfully!');
                    },
                    (err) => {
                        this.toastr.error(err?.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    AddStudentClass = (studentClass: StudentClass) => {
        Swal.fire({
            title: `${this.studentClassFormComponent.action == 'edit' ? 'Update' : 'Add'} Student class record?`,
            text: `Confirm if you want to ${
                this.studentClassFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } student class.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentClassFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentClass(studentClass);
                if (this.studentClassFormComponent.action == 'edit')
                    app.id = studentClass.id;
                let reqToProcess =
                    this.studentClassFormComponent.action == 'edit'
                        ? this.studentClassSvc.update('/studentClasses', app)
                        : this.studentClassSvc.create('/studentClasses', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.toastr.success('Student class saved successfully');
                        this.studentClassFormComponent.closeButton.nativeElement.click();
                        this.loadStudentClasses();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
