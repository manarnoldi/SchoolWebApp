import {Status} from '@/core/enums/status';
import {StaffAttendancesReportDetailsService} from '@/reports/services/staff-reports/staff-attendances-report-details.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {staffReportDefaults} from '../staff-report-defaults';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';

@Component({
    selector: 'app-staff-attendance-details-report',
    templateUrl: './staff-attendance-details-report.component.html',
    styleUrl: './staff-attendance-details-report.component.scss'
})
export class StaffAttendanceDetailsReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    staffAttendances: StaffAttendance[] = [];
    staffCategories: StaffCategory[] = [];
    staffDetails: StaffDetails[] = [];
    months: number[];
    years: number[];

    sCategoryId: number;
    statusId: number;

    currentRptMonth: number;
    currentRptYear: string;
    currentRptStatus: Status;
    currentRptStaffCategory: string;
    // The staff member whose register shows on the right, highlighted in the list.
    selectedStaffId: number | null = null;

    constructor(
        private toastr: ToastrService,
        private staffDetailsSvc: StaffDetailsService,
        private acadYearsSvc: AcademicYearsService,
        private staffAttendancesRptDetailsSvc: StaffAttendancesReportDetailsService,
        private staffAttendancesSvc: StaffAttendancesService,
        private staffCategoriesSvc: StaffCategoriesService,
        private schoolSvc: SchoolDetailsService,
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
                this.years = [
                    ...new Set(
                        academicYears.map((yr) =>
                            new Date(yr.startDate).getFullYear()
                        )
                    )
                ].sort((a, b) => b - a);
                this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
                this.restoreFromUrl();
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    // Open on the filters in the URL (refresh / shared link), else on active
    // staff of the first category with staff, in the month of its latest
    // attendance - then search and open the first staff member's register.
    private restoreFromUrl = () => {
        let form = this.ssFilterFormComponent?.schoolSoftFilterForm;
        if (!form) return;
        let urlCategory = readQueryParam(this.route, 'staffCategoryId');
        let defaults$ = urlCategory
            ? of({
                  staffCategoryId: matchOptionId(this.staffCategories, urlCategory),
                  month: +readQueryParam(this.route, 'month') || null,
                  year: +readQueryParam(this.route, 'year') || null
              })
            : staffReportDefaults(this.staffDetailsSvc, this.staffAttendancesSvc, this.staffCategories, true);
        defaults$.subscribe((d) => {
            let status = readQueryParam(this.route, 'status');
            form.patchValue({
                staffCategoryId: d.staffCategoryId,
                status: status != null && status !== '' ? +status : Status.Active,
                month: d.month,
                year: d.year
            });
            let staffId = readQueryParam(this.route, 'staffId');
            this.searchForDataMethod(form.value, (staff) => {
                let pick = (staffId && staff.find((s) => String(s.id) === staffId)) || staff[0];
                if (pick) this.staffClicked(parseInt(pick.id));
            });
        });
    };

    private syncUrl = (params: Record<string, unknown>) => pushQueryParams(this.router, this.route, params);

    yearChanged = (year: number) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.currentRptYear = year?.toString();
        this.syncUrl({year, staffId: null});
    };

    monthChanged = (month: number) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.currentRptMonth = month;
        this.syncUrl({month, staffId: null});
    };

    statusChanged = (status: Status) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.syncUrl({status, staffId: null});
    };

    staffCategoryChanged = (sCategoryId: number) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.selectedStaffId = null;
        this.syncUrl({staffCategoryId: sCategoryId, staffId: null});
    };

    staffClicked = (staffId: number) => {
        this.staffAttendances = [];
        this.selectedStaffId = staffId;
        this.syncUrl({staffId});
        this.staffAttendancesSvc
            .searchStaffAttendancesObservable(
                staffId,
                this.currentRptMonth,
                parseInt(this.currentRptYear)
            )
            .subscribe({
                next: (staffAttends) => {
                    this.staffAttendances = staffAttends;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    get selectedCount(): number {
        return this.staffDetails.filter((i) => i.isSelected).length;
    }

    // `then` gets the staff list once it loads (used when opening the page).
    searchForDataMethod = (saSearch: SchoolSoftFilter, then?: (staff: StaffDetails[]) => void) => {
        this.staffAttendances = [];
        this.selectedStaffId = null;
        if (!saSearch.staffCategoryId || saSearch.staffCategoryId == null) {
            this.toastr.error('Select staff category before clicking search!');
            return;
        } else if (!saSearch.status == null) {
            this.toastr.error(
                'Select staff status first before clicking search!'
            );
            return;
        } else if (!saSearch.month || saSearch.month == null) {
            this.toastr.error('Select month before clicking search!');
            return;
        } else if (!saSearch.year || saSearch.year == null) {
            this.toastr.error('Select year before clicking search!');
            return;
        }

        this.currentRptMonth = saSearch.month;
        this.currentRptYear = saSearch.year.toString();
        this.currentRptStatus = saSearch.status;
        this.currentRptStaffCategory = this.staffCategories.find(
            (s) => s.id == saSearch.staffCategoryId.toString()
        ).name;

        this.staffDetailsSvc
            .getBySearchDetails(saSearch.status, null, saSearch.staffCategoryId)
            .subscribe({
                next: (staffDetails) => {
                    this.staffDetails = staffDetails;
                    this.syncUrl({
                        staffCategoryId: saSearch.staffCategoryId,
                        status: saSearch.status,
                        month: saSearch.month,
                        year: saSearch.year
                    });
                    then?.(staffDetails);
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
                    Status[this.currentRptStatus].toUpperCase() +
                    ' ' +
                    this.currentRptStaffCategory.toLocaleUpperCase() +
                    ' STAFF ATTENDANCE REPORT FOR ' +
                    new Date(0, this.currentRptMonth - 1)
                        .toLocaleString('default', {
                            month: 'long'
                        })
                        .toUpperCase() +
                    ' ' +
                    this.currentRptYear.toUpperCase();

                const staffAttendRequests = this.staffDetails
                    .filter((s) => s.isSelected)
                    .map((s) =>
                        this.staffAttendancesSvc.searchStaffAttendancesObservable(
                            parseInt(s.id),
                            this.currentRptMonth,
                            parseInt(this.currentRptYear)
                        )
                    );

                forkJoin(staffAttendRequests).subscribe({
                    next: (staffAttendances) => {
                        this.staffAttendancesRptDetailsSvc.printByBatch(
                            staffAttendances,
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
