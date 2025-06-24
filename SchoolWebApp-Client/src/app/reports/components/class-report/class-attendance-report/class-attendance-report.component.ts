import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {Status} from '@/core/enums/status';
import {StudentAttendancesReport} from '@/reports/models/student-attendance-report';
import {StudentsAttendanceReportDetailsService} from '@/reports/services/class-reports/students-attendance-report-details.service';
import {StudentsAttendanceReportService} from '@/reports/services/class-reports/students-attendance-report.service';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StudentAttendancesService} from '@/students/services/student-attendances.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-class-attendance-report',
    templateUrl: './class-attendance-report.component.html',
    styleUrl: './class-attendance-report.component.scss'
})
export class ClassAttendanceReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    studentAttendancesRpt: StudentAttendancesReport[] = [];
    schoolClasses: SchoolClass[] = [];

    months: number[];
    academicYears: AcademicYear[];

    currentRptMonth: number;
    currentRptYear: string;
    currentRptStatus: Status;
    currentRptSchoolClass: string;

    constructor(
        private toastr: ToastrService,
        private schoolClassesSvc: SchoolClassesService,
        private acadYearsSvc: AcademicYearsService,
        private studentAttendsRptSvc: StudentsAttendanceReportService,
        private studentAttendancesSvc: StudentAttendancesService,
        private saRSvc: StudentsAttendanceReportDetailsService,
        private schoolSvc: SchoolDetailsService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    loadInitials = () => {
        this.acadYearsSvc.get('/academicYears').subscribe({
            next: (academicYears) => {
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

    statusChanged = (status: Status) => {
        this.resetControlls();
    };

    academicYearChanged = (acadYearId: number) => {
        this.resetControlls();
        this.schoolClassesSvc.getByAcademicYearId(acadYearId).subscribe({
            next: (schoolClasses) => {
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
                this.ssFilterFormComponent.schoolSoftFilterForm
                    .get('schoolClassId')
                    .reset();
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    monthChangedChanged = (employTypeId: number) => {
        this.resetControlls();
    };
    schoolClassChanged = (sCategoryId: number) => {
        this.resetControlls();
    };

    resetControlls = () => {
        this.studentAttendancesRpt = [];
        this.currentRptMonth = 0;
        this.currentRptYear = '';
        this.currentRptSchoolClass = '';
    };

    searchForDataMethod = (saSearch: SchoolSoftFilter) => {
        this.studentAttendancesRpt = [];
        if (!saSearch.academicYearId || saSearch.academicYearId == null) {
            this.toastr.error('Select year before clicking search!');
            return;
        } else if (!saSearch.schoolClassId || saSearch.schoolClassId == null) {
            this.toastr.error('Select school class before clicking search!');
            return;
        } else if (saSearch.status == null) {
            this.toastr.error('Select staff status before clicking search!');
            return;
        } else if (!saSearch.month || saSearch.month == null) {
            this.toastr.error('Select month before clicking search!');
            return;
        }
        this.studentAttendsRptSvc
            .getStudentAttendancesReport(
                saSearch.month,
                saSearch.schoolClassId,
                saSearch.status
            )
            .subscribe({
                next: (studentAttendsRpt) => {
                    this.currentRptMonth = saSearch.month;
                    this.currentRptYear = this.academicYears.find(
                        (a) => a.id == saSearch.academicYearId.toString()
                    ).name;
                    this.currentRptStatus = saSearch.status;
                    this.currentRptSchoolClass = this.schoolClasses.find(
                        (s) => s.id == saSearch.schoolClassId.toString()
                    ).name;
                    this.studentAttendancesRpt = studentAttendsRpt;

                    if (studentAttendsRpt.length <= 0) {
                        this.toastr.info('No student assigned to the class!');
                    }
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    printIndividualReport = (studentClassId: number) => {
        let schDetailsReq = this.schoolSvc.get('/schooldetails');
        let studentAttendReq =
            this.studentAttendancesSvc.searchStudentAttendancesObservable(
                studentClassId,
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
                        .toUpperCase() 
                const staffAttends = [];
                staffAttends.push(attends);

                this.saRSvc.printByBatch(staffAttends, school[0], reportTitle);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    printReport = () => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                let reportTitle =
                    Status[this.currentRptStatus].toUpperCase() +
                    ' ' +
                    this.currentRptMonth.toString().toLocaleUpperCase() +
                    ' STUDENT ATTENDANCE REPORT FOR ' +
                    new Date(0, this.currentRptMonth - 1)
                        .toLocaleString('default', {month: 'long'})
                        .toUpperCase() +
                    ' ' +
                    this.currentRptYear.toLocaleUpperCase();
                this.studentAttendsRptSvc.generateReport(
                    school[0],
                    this.studentAttendancesRpt,
                    reportTitle
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };
}
