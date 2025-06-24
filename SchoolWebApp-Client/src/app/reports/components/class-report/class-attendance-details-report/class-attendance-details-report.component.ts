import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {Status} from '@/core/enums/status';
import {StudentsAttendanceReportDetailsService} from '@/reports/services/class-reports/students-attendance-report-details.service';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StudentAttendance} from '@/students/models/student-attendance';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {StudentAttendancesService} from '@/students/services/student-attendances.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-class-attendance-details-report',
    templateUrl: './class-attendance-details-report.component.html',
    styleUrl: './class-attendance-details-report.component.scss'
})
export class ClassAttendanceDetailsReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    studentAttendances: StudentAttendance[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];
    months: number[];
    academicYears: AcademicYear[] = [];

    sSchoolClassId: number;
    statusId: number;

    currentRptMonth: number;
    currentRptStatus: Status;
    currentRptSchoolClass: string;
    currentStudentClassId: number;

    constructor(
        private toastr: ToastrService,
        private studentClassSvc: StudentClassService,
        private studentClassesSvc: StudentClassService,
        private saRptSvc: StudentsAttendanceReportDetailsService,
        private studentAttendancesSvc: StudentAttendancesService,
        private schoolClassesSvc: SchoolClassesService,
        private academicYearsSvc: AcademicYearsService,
        private schoolSvc: SchoolDetailsService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    loadInitials = () => {
        let acadYearReq = this.academicYearsSvc.get('/academicYears');
        forkJoin([acadYearReq]).subscribe({
            next: ([academicYears]) => {
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    academicYearChanged = (acadYearId: number) => {
        this.schoolClasses = [];
        this.studentAttendances = [];
        this.studentClasses = [];
        this.schoolClassesSvc.getByAcademicYearId(acadYearId).subscribe({
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

    monthChanged = (month: number) => {
        this.studentAttendances = [];
        this.studentClasses = [];
        this.currentRptMonth = month;
        this.currentStudentClassId = null;
    };

    statusChanged = (status: Status) => {
        this.studentAttendances = [];
        this.studentClasses = [];
        this.currentStudentClassId = null;
    };

    schoolClassChanged = (sClassId: number) => {
        this.studentAttendances = [];
        this.studentClasses = [];
        this.currentStudentClassId = null;
    };

    studentClassClicked = (studentClassId: number) => {
        this.studentAttendances = [];
        this.currentStudentClassId = null;
        this.studentAttendancesSvc
            .getByMonthStudentClassId(this.currentRptMonth, studentClassId)
            .subscribe({
                next: (studentAttends) => {
                    this.studentAttendances = studentAttends;
                    this.currentStudentClassId = studentClassId;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    get selectedCount(): number {
        return this.studentClasses.filter((i) => i.isSelected).length;
    }

    searchForDataMethod = (saSearch: SchoolSoftFilter) => {
        this.studentAttendances = [];
        this.studentClasses = [];
        this.currentStudentClassId = null;

        if (!saSearch.academicYearId || saSearch.academicYearId == null) {
            this.toastr.error('Select academic year before clicking search!');
            return;
        } else if (!saSearch.schoolClassId || saSearch.schoolClassId == null) {
            this.toastr.error('Select the class before clicking search!');
            return;
        } else if (!saSearch.status == null) {
            this.toastr.error(
                'Select staff status first before clicking search!'
            );
            return;
        } else if (!saSearch.month || saSearch.month == null) {
            this.toastr.error('Select month before clicking search!');
            return;
        }

        this.currentRptMonth = this.months.find(
            (m) => m == this.currentRptMonth
        );
        this.currentRptStatus = saSearch.status;
        this.currentRptSchoolClass = this.schoolClasses.find(
            (s) => s.id == saSearch.schoolClassId.toString()
        ).name;

        this.studentClassesSvc
            .getBySchoolClassId(saSearch.schoolClassId, saSearch.status)
            .subscribe({
                next: (studentClasses) => {
                    this.studentClasses = studentClasses.sort(
                        (a, b) => b.schoolClass?.rank - a.schoolClass?.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    printReport = () => {
        let schDetailsReq = this.schoolSvc.get('/schooldetails');
        let studentAttendReq =
            this.studentAttendancesSvc.searchStudentAttendancesObservable(
                this.currentStudentClassId,
                this.currentRptMonth
            );
        forkJoin([schDetailsReq, studentAttendReq]).subscribe({
            next: ([school, attends]) => {
                const reportTitle =
                    Status[this.currentRptStatus].toUpperCase() +
                    ' STUDENT ATTENDANCE REPORT FOR ' +
                    this.currentRptSchoolClass.toLocaleUpperCase() +
                    ' ' +
                    new Date(0, this.currentRptMonth - 1)
                        .toLocaleString('default', {
                            month: 'long'
                        })
                        .toUpperCase();

                const studAttendRequests = this.studentClasses
                    .filter((s) => s.isSelected)
                    .map((s) =>
                        this.studentAttendancesSvc.searchStudentAttendancesObservable(
                            parseInt(s.id),
                            this.currentRptMonth
                        )
                    );

                forkJoin(studAttendRequests).subscribe({
                    next: (studAttends) => {
                        this.saRptSvc.printByBatch(
                            studAttends,
                            school[0],
                            reportTitle
                        );
                    },
                    error: (err) => {
                        this.toastr.error(err.error || err);
                    }
                });
            },
            error: (err) => {
                this.toastr.error(err.error || err);
            }
        });
    };
}
