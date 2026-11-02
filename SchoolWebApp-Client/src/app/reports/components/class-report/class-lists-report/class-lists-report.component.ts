import {ClassLeadership} from '@/class/models/class-leadership';
import {SchoolClass} from '@/class/models/school-class';
import {ClassLeadershipsService} from '@/class/services/class-leaderships.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {Status} from '@/core/enums/status';
import {ClassListReportService} from '@/reports/services/class-reports/class-list-report.service';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StudentClass} from '@/students/models/student-class';
import {StudentClassService} from '@/students/services/student-class.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {currentYear, firstOf} from '@/core/utils/current-defaults';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-class-lists-report',
    templateUrl: './class-lists-report.component.html',
    styleUrl: './class-lists-report.component.scss'
})
export class ClassListsReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    academicYears: AcademicYear[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];

    schoolClassLeaders: ClassLeadership[] = [];

    currentStatus: Status = Status.Active;

    constructor(
        private toastr: ToastrService,
        private acadYearsSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private schoolSvc: SchoolDetailsService,
        private classListRptSvc: ClassListReportService,
        private classLeadershipsSvc: ClassLeadershipsService,
        private router: Router,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    get selectedCount(): number {
        return this.schoolClasses.filter((i) => i.isSelected).length;
    }

    private syncUrl = (params: Record<string, unknown>) => pushQueryParams(this.router, this.route, params);

    // Open on the filters in the URL (refresh / shared link), else on the
    // current year's active learners - then list the classes and open the
    // first one, so the page never opens empty.
    private restoreFromUrl = () => {
        let form = this.ssFilterFormComponent?.schoolSoftFilterForm;
        if (!form) return;
        let urlYear = readQueryParam(this.route, 'academicYearId');
        let year = urlYear ? matchOptionId(this.academicYears, urlYear) : currentYear(this.academicYears)?.id;
        if (year == null) return;
        let urlStatus = readQueryParam(this.route, 'status');
        let status = urlStatus != null && urlStatus !== '' ? +urlStatus : Status.Active;
        form.patchValue({academicYearId: year, status});
        this.currentStatus = status;
        this.searchForDataMethod(new SchoolSoftFilter(form.value), () => {
            let classId = urlYear
                ? matchOptionId(this.schoolClasses, readQueryParam(this.route, 'schoolClassId'))
                : firstOf(this.schoolClasses)?.id;
            if (classId != null) this.schoolClassClicked(classId);
        });
    };

    schoolClassChanged = (classId: number) => {
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
    };

    schoolClassClicked = (schoolClassId: number) => {
        this.studentClasses = [];
        this.schoolClassLeaders = [];
        this.syncUrl({schoolClassId});

        let scLeadersReq =
            this.classLeadershipsSvc.getBySchoolClassId(schoolClassId);
        let scBySchoolClassIdReq = this.studentClassesSvc.getBySchoolClassId(
            schoolClassId,
            this.currentStatus
        );

        forkJoin([scLeadersReq, scBySchoolClassIdReq]).subscribe({
            next: ([scLeaders, studentClasses]) => {
                this.schoolClassLeaders = scLeaders;
                this.studentClasses = studentClasses.sort((a, b) =>
                    a.student?.upi.localeCompare(b.student?.upi)
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    statusChanged = (status: Status) => {
        this.currentStatus = status;
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
        this.syncUrl({status, schoolClassId: null});
    };

    academicYearChanged = (academicYearId: number) => {
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
        this.syncUrl({academicYearId, schoolClassId: null});
    };

    // `then` runs once the classes are in (used when restoring).
    searchForDataMethod = (ssFilter: SchoolSoftFilter, then?: () => void) => {
        if (!ssFilter.academicYearId || ssFilter.academicYearId == null) {
            this.toastr.error('Select academic year before clicking search!');
            return;
        } else if (!ssFilter.status == null) {
            this.toastr.error('Select status before clicking search!');
            return;
        }
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
        this.syncUrl({academicYearId: ssFilter.academicYearId, status: ssFilter.status});
        this.schoolClassesSvc
            .getByAcademicYearId(ssFilter.academicYearId)
            .subscribe({
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

    printReport = () => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                const status =
                    this.ssFilterFormComponent.schoolSoftFilterForm.get(
                        'status'
                    ).value;

                const reportTitle = 'SCHOOL CLASS LIST REPORT';
                const classListsRequests = this.schoolClasses
                    .filter((s) => s.isSelected)
                    .map((s) =>
                        this.studentClassesSvc.getBySchoolClassId(
                            parseInt(s.id),
                            status
                        )
                    );

                forkJoin(classListsRequests).subscribe({
                    next: (studentClass) => {
                        this.classListRptSvc.printByBatch(
                            studentClass.filter((s) => s.length > 0),
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

    loadInitials = () => {
        this.acadYearsSvc.get('/academicYears').subscribe({
            next: (academicYears) => {
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.restoreFromUrl();
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };
}
