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
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {catchError, map, switchMap} from 'rxjs/operators';
import {currentYear, firstOf} from '@/core/utils/current-defaults';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';

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
        private schoolSvc: SchoolDetailsService,
        private router: Router,
        private route: ActivatedRoute
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
                this.restoreFromUrl();
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    private syncUrl = (params: Record<string, unknown>) => pushQueryParams(this.router, this.route, params);

    // Open on the filters in the URL (refresh / shared link), else on the
    // current year's first class, active learners and the month of the
    // class's latest attendance - then search and open the first learner, so
    // the page never opens empty.
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
                this.currentRptMonth = month;
                this.searchForDataMethod(new SchoolSoftFilter(form.value), () => {
                    let studentClassId = urlYear
                        ? matchOptionId(this.studentClasses, readQueryParam(this.route, 'studentClassId'))
                        : firstOf(this.studentClasses)?.id;
                    if (studentClassId != null) this.studentClassClicked(studentClassId);
                });
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

    // `syncUrl` is off while restoring; `then` runs once the classes are in.
    academicYearChanged = (acadYearId: number, syncUrl = true, then?: () => void) => {
        this.schoolClasses = [];
        this.studentAttendances = [];
        this.studentClasses = [];
        if (syncUrl) this.syncUrl({academicYearId: acadYearId, schoolClassId: null, studentClassId: null});
        this.schoolClassesSvc.getByAcademicYearId(acadYearId).subscribe({
            next: (schoolClasses) => {
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
                then?.();
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
        this.syncUrl({month, studentClassId: null});
    };

    statusChanged = (status: Status) => {
        this.studentAttendances = [];
        this.studentClasses = [];
        this.currentStudentClassId = null;
        this.syncUrl({status, studentClassId: null});
    };

    schoolClassChanged = (sClassId: number) => {
        this.studentAttendances = [];
        this.studentClasses = [];
        this.currentStudentClassId = null;
        this.syncUrl({schoolClassId: sClassId, studentClassId: null});
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
                    this.syncUrl({studentClassId});
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    get selectedCount(): number {
        return this.studentClasses.filter((i) => i.isSelected).length;
    }

    // `then` runs once the learners are in (used when restoring).
    searchForDataMethod = (saSearch: SchoolSoftFilter, then?: () => void) => {
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
        this.syncUrl({
            academicYearId: saSearch.academicYearId,
            schoolClassId: saSearch.schoolClassId,
            status: saSearch.status,
            month: saSearch.month
        });

        this.studentClassesSvc
            .getBySchoolClassId(saSearch.schoolClassId, saSearch.status)
            .subscribe({
                next: (studentClasses) => {
                    this.studentClasses = studentClasses.sort(
                        (a, b) => b.schoolClass?.rank - a.schoolClass?.rank
                    );
                    then?.();
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
