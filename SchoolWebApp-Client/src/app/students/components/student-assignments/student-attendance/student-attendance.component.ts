import {StudentDetails} from '@/students/models/student-details';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {StudentAttendanceFormComponent} from './student-attendance-form/student-attendance-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StudentAttendance} from '@/students/models/student-attendance';
import {ToastrService} from 'ngx-toastr';
import {StudentAttendancesService} from '@/students/services/student-attendances.service';
import {ActivatedRoute} from '@angular/router';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SchoolClass} from '@/class/models/school-class';
import {StudentClass} from '@/students/models/student-class';
import {StudentClassService} from '@/students/services/student-class.service';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import { SchoolSoftFilter } from '@/shared/models/school-soft-filter';

@Component({
    selector: 'app-student-attendance',
    templateUrl: './student-attendance.component.html',
    styleUrl: './student-attendance.component.scss'
})
export class StudentAttendanceComponent implements OnInit {
    @Input() statuses;
    @Input() student: StudentDetails;
    @ViewChild(StudentAttendanceFormComponent)
    studentAttendanceFormComponent: StudentAttendanceFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    today = new Date();
    studentId: number = 0;
    studentAttendance: StudentAttendance;
    studentAttendances: StudentAttendance[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[];
    months: number[];
    years: number[];

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private studentAttendancesSvc: StudentAttendancesService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStudentAttendances();
    }

    monthChanged = (month: number) => {
        this.studentAttendances = [];
    };

    yearChanged = (year: number) => {
        this.studentAttendances = [];
    };

    schoolClassChanged = (sci: number) => {
        this.studentAttendances = [];
    };

    // loadStudentAttendances = () => {
    //     this.route.queryParams.subscribe((params) => {
    //         this.studentId = params['id'];
    //         let attendanceByStudentIdReq = this.studentAttendancesSvc.get(
    //             '/studentAttendances/byStudentId/' + this.studentId.toString()
    //         );
    //         let schoolClassesReq = this.schoolClassesSvc.get('/schoolClasses');

    //         forkJoin([attendanceByStudentIdReq, schoolClassesReq]).subscribe(
    //             ([studentAttendances, schoolClasses]) => {
    //                 this.studentAttendances = studentAttendances;
    //                 this.schoolClasses = schoolClasses;
    //             },
    //             (err) => {
    //                 this.toastr.error(err.error);
    //             }
    //         );
    //     });
    // };

    searchStudentAttendances = (dmy: SchoolSoftFilter) => {
        if (!dmy.studentClassId) {
            this.toastr.error(
                'The student class selected is not valid/correct!'
            );
            return;
        } else if (!dmy.month || dmy.month < 1 || dmy.month > 12) {
            this.toastr.error('The month selected is not valid/correct!');
            return;
        } else if (!dmy.year) {
            this.toastr.error('The year selected is not valid/correct!');
            return;
        }

        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            this.studentAttendancesSvc
                .getByMonthYearSchoolClassId(
                    dmy.month,
                    dmy.year,
                    dmy.studentClassId
                )
                .subscribe({
                    next: (studAttends) => {
                        this.studentAttendances = studAttends.sort(
                            (a, b) =>
                                new Date(a?.date ?? '').getTime() -
                                new Date(b?.date ?? '').getTime()
                        );
                        if (
                            this.studentAttendances.length <= 0 &&
                            !this.firstLoad
                        ) {
                            this.toastr.info(
                                'No student attendance record/s found for the search parameters!'
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

    // schoolClassChanged = (acadYearId: number) => {
    //     if (!acadYearId) {
    //         this.toastr.error(
    //             'The academic year selected is not valid/correct!'
    //         );
    //         return;
    //     }

    //     this.schoolClassesSvc
    //         .get('/schoolClasses/byAcademicYearId/1')
    //         .subscribe({
    //             next: (schoolClasses) => {
    //                 this.schoolClasses = schoolClasses.sort((a, b) => a.rank - b.rank);;
    //             },
    //             error: (err) => {
    //                 this.toastr.error(err.error);
    //             }
    //         });
    // };

    loadStudentAttendances = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            let monthsReq = this.studentAttendancesSvc.getDistinctMonths();
            let yearsReq = this.studentAttendancesSvc.getDistinctYears();
            let schoolClassesReq = this.schoolClassesSvc.get('/schoolClasses');
            let studentClassesReq = this.studentClassesSvc.get(
                '/studentClasses/byStudentId/' + this.studentId
            );

            forkJoin([
                monthsReq,
                yearsReq,
                schoolClassesReq,
                studentClassesReq
            ]).subscribe({
                next: ([months, years, schoolClasses, studentClasses]) => {
                    this.months = months;
                    this.years = years;
                    this.schoolClasses = schoolClasses.sort(
                        (a, b) => a.rank - b.rank
                    );
                    this.studentClasses = studentClasses.sort(
                        (a, b) =>
                            b.schoolClass?.academicYear.rank -
                            a.schoolClass?.academicYear.rank
                    );

                    const topMonth = this.today.getMonth() + 1;                    
                    const topStudClass = this.studentClasses[0];
                    const topYear = new Date(
                        topStudClass.schoolClass?.academicYear?.startDate
                    ).getFullYear();

                    let dmy = new SchoolSoftFilter();
                    dmy.month = topMonth;
                    dmy.year = topYear;
                    dmy.studentClassId = parseInt(topStudClass.id);

                    this.ssFilterFormComponent.setFormControls(dmy);

                    this.searchStudentAttendances(dmy);
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
        });
    };

    editItem(id: number) {
        this.studentAttendancesSvc.getById(id, '/studentAttendances').subscribe(
            (res) => {
                let studentAttendanceId = res.id;
                this.studentAttendance = new StudentAttendance(res);
                this.studentAttendance.id = studentAttendanceId;
                this.studentAttendanceFormComponent.setFormControls(
                    this.studentAttendance
                );
                this.studentAttendanceFormComponent.editMode = true;
                this.studentAttendanceFormComponent.studentAttendance =
                    this.studentAttendance;
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
                this.studentAttendancesSvc
                    .delete('/studentAttendances', id)
                    .subscribe(
                        (res) => {
                            this.loadStudentAttendances();
                            this.toastr.success('Record deleted successfully!');
                        },
                        (err) => {
                            this.toastr.error(err);
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    AddStudentAttendance = (studentAttendance: StudentAttendance) => {
        Swal.fire({
            title: `${this.studentAttendanceFormComponent.editMode ? 'Update' : 'Add'} Student attendance record?`,
            text: `Confirm if you want to ${
                this.studentAttendanceFormComponent.editMode ? 'update' : 'add'
            } student attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.studentAttendanceFormComponent.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StudentAttendance(studentAttendance);
                if (this.studentAttendanceFormComponent.editMode)
                    app.id = studentAttendance.id;
                let reqToProcess = this.studentAttendanceFormComponent.editMode
                    ? this.studentAttendancesSvc.update(
                          '/studentAttendances',
                          app
                      )
                    : this.studentAttendancesSvc.create(
                          '/studentAttendances',
                          app
                      );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.studentAttendanceFormComponent.editMode = false;
                        this.toastr.success(
                            'Student attendance saved successfully'
                        );
                        this.studentAttendanceFormComponent.closeButton.nativeElement.click();
                        this.studentAttendanceFormComponent.setFormControls(
                            null
                        );
                        this.loadStudentAttendances();
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
