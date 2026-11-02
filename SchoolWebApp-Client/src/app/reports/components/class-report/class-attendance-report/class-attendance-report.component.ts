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
import {StudentClassService} from '@/students/services/student-class.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {catchError, map, switchMap} from 'rxjs/operators';
import {currentYear, firstOf} from '@/core/utils/current-defaults';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';

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
        private schoolSvc: SchoolDetailsService,
        private studentClassSvc: StudentClassService,
        private router: Router,
        private route: ActivatedRoute
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
                this.restoreFromUrl();
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    private syncUrl = (params: Record<string, unknown>) => pushQueryParams(this.router, this.route, params);

    // Open on the filters in the URL (refresh / shared link), else on the
    // current year's first class, active learners, and the month of the
    // class's latest attendance - then search, so the page never opens empty.
    private restoreFromUrl = () => {
        let form = this.ssFilterFormComponent?.schoolSoftFilterForm;
        if (!form) return;
        let urlYear = readQueryParam(this.route, 'academicYearId');
        let year = urlYear ? matchOptionId(this.academicYears, urlYear) : currentYear(this.academicYears)?.id;
        if (year == null) return;
        let urlStatus = readQueryParam(this.route, 'status');
        let status = urlStatus != null && urlStatus !== '' ? +urlStatus : Status.Active;
        form.patchValue({academicYearId: year, status});
        this.academicYearChanged(year, false, () => {
            let classId = urlYear
                ? matchOptionId(this.schoolClasses, readQueryParam(this.route, 'schoolClassId'))
                : firstOf(this.schoolClasses)?.id;
            if (classId == null) return;
            form.patchValue({schoolClassId: classId});
            let urlMonth = readQueryParam(this.route, 'month');
            let month$ = urlYear && urlMonth ? of(+urlMonth) : this.latestAttendanceMonth(classId);
            month$.subscribe((month) => {
                form.patchValue({month});
                this.searchForDataMethod(new SchoolSoftFilter(form.value));
            });
        });
    };

    // The month of the class's most recent attendance up to today, read off
    // its first learner's register; else this month.
    private latestAttendanceMonth = (schoolClassId: any) => {
        let now = new Date();
        let today = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
        return this.studentClassSvc.getBySchoolClassId(schoolClassId, Status.Active).pipe(
            switchMap((studentClasses) =>
                studentClasses.length
                    ? this.studentAttendancesSvc.get('/studentAttendances/byStudentClassId/' + studentClasses[0].id)
                    : of([])
            ),
            map((records: any[]) => {
                let days = (records || [])
                    .map((r) => String(r.date ?? '').substring(0, 10))
                    .filter((d) => d && d <= today)
                    .sort();
                return days.length ? +days[days.length - 1].substring(5, 7) : now.getMonth() + 1;
            }),
            catchError(() => of(now.getMonth() + 1))
        );
    };

    statusChanged = (status: Status) => {
        this.resetControlls();
        this.syncUrl({status});
    };

    // `syncUrl` is off while restoring; `then` runs once the classes are in.
    academicYearChanged = (acadYearId: number, syncUrl = true, then?: () => void) => {
        this.resetControlls();
        if (syncUrl) this.syncUrl({academicYearId: acadYearId, schoolClassId: null});
        this.schoolClassesSvc.getByAcademicYearId(acadYearId).subscribe({
            next: (schoolClasses) => {
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
                this.ssFilterFormComponent.schoolSoftFilterForm
                    .get('schoolClassId')
                    .reset();
                then?.();
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    monthChangedChanged = (month: number) => {
        this.resetControlls();
        this.syncUrl({month});
    };
    schoolClassChanged = (schoolClassId: number) => {
        this.resetControlls();
        this.syncUrl({schoolClassId});
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
        this.syncUrl({
            academicYearId: saSearch.academicYearId,
            schoolClassId: saSearch.schoolClassId,
            status: saSearch.status,
            month: saSearch.month
        });
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
