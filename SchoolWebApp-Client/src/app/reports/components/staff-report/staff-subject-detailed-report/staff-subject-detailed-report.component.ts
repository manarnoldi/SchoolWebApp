import {Status} from '@/core/enums/status';
import {StaffSubjectDetailedReportService} from '@/reports/services/staff-reports/staff-subject-detailed-report.service';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffSubject} from '@/staff/models/staff-subject';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {StaffSubjectsService} from '@/staff/services/staff-subjects.service';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {currentYear} from '@/core/utils/current-defaults';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {staffReportDefaults} from '../staff-report-defaults';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {map} from 'rxjs/operators';

@Component({
    selector: 'app-staff-subject-detailed-report',
    templateUrl: './staff-subject-detailed-report.component.html',
    styleUrl: './staff-subject-detailed-report.component.scss'
})
export class StaffSubjectDetailedReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    staffCategories: StaffCategory[] = [];
    staffDetails: StaffDetails[] = [];
    // staffId -> count of StaffSubject rows for the current report year.
    // Populated in parallel after searchForDataMethod resolves the staff list,
    // then bound to the min-table so each row renders a clickable count badge.
    subjectCounts: Record<number, number> = {};
    // Last-clicked staff id, bound to the min-table so the row gets a
    // table-active highlight while its subjects show in the right pane.
    selectedStaffId: number | null = null;

    staffSubjects: StaffSubject[];

    academicYears: AcademicYear[];

    sCategoryId: number;
    statusId: number;

    currentRptYearId: number;
    currentRptStatus: Status;
    currentRptStaffCategory: string;

    constructor(
        private toastr: ToastrService,
        private staffDetailsSvc: StaffDetailsService,
        private acadYearsSvc: AcademicYearsService,
        private staffSubjectsSvc: StaffSubjectsService,
        private staffCategoriesSvc: StaffCategoriesService,
        private staffSubjectDetailsRptSvc: StaffSubjectDetailedReportService,
        private schoolSvc: SchoolDetailsService,
        private staffAttendancesSvc: StaffAttendancesService,
        private router: Router,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    loadInitials = () => {
        let staffCatReq = this.staffCategoriesSvc.get('/staffCategories');
        let acadYearReq = this.acadYearsSvc.get('/academicYears');

        forkJoin([staffCatReq, acadYearReq]).subscribe({
            next: ([staffCategories, academicYears]) => {
                this.staffCategories = staffCategories;
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

    // Open on the filters in the URL (refresh / shared link), else on active
    // staff of the first category with staff in the current year - then
    // search and open the first staff member who teaches something.
    private restoreFromUrl = () => {
        let form = this.ssFilterFormComponent?.schoolSoftFilterForm;
        if (!form) return;
        let urlCategory = readQueryParam(this.route, 'staffCategoryId');
        let category$ = urlCategory
            ? of(matchOptionId(this.staffCategories, urlCategory))
            : staffReportDefaults(this.staffDetailsSvc, this.staffAttendancesSvc, this.staffCategories, false).pipe(
                  map((d) => d.staffCategoryId)
              );
        category$.subscribe((staffCategoryId) => {
            let status = readQueryParam(this.route, 'status');
            let yearId = urlCategory
                ? matchOptionId(this.academicYears, readQueryParam(this.route, 'academicYearId'))
                : currentYear(this.academicYears)?.id;
            form.patchValue({
                staffCategoryId,
                status: status != null && status !== '' ? +status : Status.Active,
                academicYearId: yearId ?? null
            });
            let staffId = readQueryParam(this.route, 'staffId');
            this.searchForDataMethod(form.value, (staff, counts) => {
                let pick =
                    (staffId && staff.find((s) => String(s.id) === staffId)) ||
                    staff.find((s) => counts[parseInt(s.id)] > 0) ||
                    staff[0];
                if (pick) this.staffClicked(parseInt(pick.id));
            });
        });
    };

    private syncUrl = (params: Record<string, unknown>) => pushQueryParams(this.router, this.route, params);

    academicYearChanged = (academicYearChanged: number) => {
        this.currentRptYearId = academicYearChanged;
        this.staffSubjects = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.syncUrl({academicYearId: academicYearChanged, staffId: null});
    };

    statusChanged = (status: Status) => {
        this.staffSubjects = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.syncUrl({status, staffId: null});
    };

    staffCategoryChanged = (sCategoryId: number) => {
        this.staffSubjects = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.syncUrl({staffCategoryId: sCategoryId, staffId: null});
    };

    staffClicked = (staffId: number) => {
        this.selectedStaffId = staffId;
        this.syncUrl({staffId});
        this.staffSubjects = [];
        this.staffSubjectsSvc
            .getByStaffYearId(staffId, this.currentRptYearId)
            .subscribe({
                next: (staffSubjects) => {
                    this.staffSubjects = staffSubjects.sort(
                        (a, b) => a.schoolClass?.rank - b.schoolClass?.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    get selectedCount(): number {
        return this.staffDetails.filter((i) => i.isSelected).length;
    }

    // `then` gets the staff list and their subject counts once both load
    // (used when opening the page).
    searchForDataMethod = (
        saSearch: SchoolSoftFilter,
        then?: (staff: StaffDetails[], counts: Record<number, number>) => void
    ) => {
        this.staffSubjects = [];
        this.selectedStaffId = null;
        if (!saSearch.staffCategoryId || saSearch.staffCategoryId == null) {
            this.toastr.error('Select staff category before clicking search!');
            return;
        } else if (!saSearch.status == null) {
            this.toastr.error(
                'Select staff status first before clicking search!'
            );
            return;
        } else if (
            !saSearch.academicYearId ||
            saSearch.academicYearId == null
        ) {
            this.toastr.error('Select academic year before clicking search!');
            return;
        }

        this.currentRptYearId = saSearch.academicYearId;
        this.currentRptStatus = saSearch.status;
        this.currentRptStaffCategory = this.staffCategories.find(
            (s) => s.id == saSearch.staffCategoryId.toString()
        ).name;

        this.staffDetailsSvc
            .getBySearchDetails(saSearch.status, null, saSearch.staffCategoryId)
            .subscribe({
                next: (staffDetails) => {
                    this.staffDetails = staffDetails;
                    this.subjectCounts = {};
                    this.syncUrl({
                        staffCategoryId: saSearch.staffCategoryId,
                        status: saSearch.status,
                        academicYearId: saSearch.academicYearId
                    });
                    if (!staffDetails?.length) then?.([], {});
                    // Eager parallel fetch of subject counts so each row can show
                    // a clickable badge with the assignment count for the year.
                    if (staffDetails?.length) {
                        const reqs = staffDetails.map((s) =>
                            this.staffSubjectsSvc.getByStaffYearId(
                                parseInt(s.id),
                                this.currentRptYearId
                            )
                        );
                        forkJoin(reqs).subscribe({
                            next: (results) => {
                                const counts: Record<number, number> = {};
                                staffDetails.forEach((s, idx) => {
                                    counts[parseInt(s.id)] = (results[idx] || []).length;
                                });
                                this.subjectCounts = counts;
                                then?.(staffDetails, counts);
                            },
                            error: (err) => {
                                // Non-fatal: badges just won't render.
                                this.toastr.warning('Could not load subject counts.');
                                then?.(staffDetails, {});
                            }
                        });
                    }
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    printReport = () => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                const reportTitle =
                    'STAFF SUBJECT ALLOCATION REPORT FOR ' +
                    this.academicYears
                        .find((i) => i.id == this.currentRptYearId.toString())
                        .name.toUpperCase();
                const staffSubjectRequests = this.staffDetails
                    .filter((s) => s.isSelected)
                    .map((s) =>
                        this.staffSubjectsSvc.getByStaffYearId(
                            parseInt(s.id),
                            this.currentRptYearId
                        )
                    );
                forkJoin(staffSubjectRequests).subscribe({
                    next: (staffSubjects) => {
                        this.staffSubjectDetailsRptSvc.printByBatch(
                            staffSubjects.filter((s) => s.length > 0),
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
