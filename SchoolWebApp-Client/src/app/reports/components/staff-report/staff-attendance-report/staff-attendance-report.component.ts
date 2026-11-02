import {Status} from '@/core/enums/status';
import {StaffAttendancesReport} from '@/reports/models/staff-attendances-report';
import {StaffAttendancesReportDetailsService} from '@/reports/services/staff-reports/staff-attendances-report-details.service';
import {StaffAttendancesReportService} from '@/reports/services/staff-reports/staff-attendances-report.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {matchOptionId, pushQueryParams, readQueryParam} from '@/shared/utils/query-param-sync';
import {staffReportDefaults} from '../staff-report-defaults';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';

@Component({
    selector: 'app-staff-attendance-report',
    templateUrl: './staff-attendance-report.component.html',
    styleUrl: './staff-attendance-report.component.scss'
})
export class StaffAttendanceReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    staffAttendancesRpt: StaffAttendancesReport[] = [];
    staffCategories: StaffCategory[] = [];

    months: number[];
    years: number[];

    currentRptMonth: number;
    currentRptYear: string;
    currentRptStatus: Status;
    currentRptStaffCategory: string;

    constructor(
        private toastr: ToastrService,
        private staffCategoriesSvc: StaffCategoriesService,
        private acadYearsSvc: AcademicYearsService,
        private staffAttendsRptSvc: StaffAttendancesReportService,
        private staffAttendancesSvc: StaffAttendancesService,
        private saRDSvc: StaffAttendancesReportDetailsService,
        private schoolSvc: SchoolDetailsService,
        private staffDetailsSvc: StaffDetailsService,
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
    // attendance - then search, so the report never opens empty.
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
            this.ssFilterFormComponent.onSubmit();
        });
    };

    private syncUrl = (params: Record<string, unknown>) => pushQueryParams(this.router, this.route, params);

    statusChanged = (status: Status) => {
        this.resetControlls();
        this.syncUrl({status});
    };

    yearChanged = (year: any) => {
        this.resetControlls();
        this.syncUrl({year});
    };

    monthChangedChanged = (month: number) => {
        this.resetControlls();
        this.syncUrl({month});
    };
    staffCategoryChanged = (sCategoryId: number) => {
        this.resetControlls();
        this.syncUrl({staffCategoryId: sCategoryId});
    };

    resetControlls = () => {
        this.staffAttendancesRpt = [];
        this.currentRptMonth = 0;
        this.currentRptYear = '';
        this.currentRptStaffCategory = '';
    };

    searchForDataMethod = (saSearch: SchoolSoftFilter) => {
        this.staffAttendancesRpt = [];
        if (!saSearch.staffCategoryId || saSearch.staffCategoryId == null) {
            this.toastr.error('Select staff category before clicking search!');
            return;
        } else if (saSearch.status == null) {
            this.toastr.error('Select staff status before clicking search!');
            return;
        } else if (!saSearch.month || saSearch.month == null) {
            this.toastr.error('Select month before clicking search!');
            return;
        } else if (!saSearch.year || saSearch.year == null) {
            this.toastr.error('Select year before clicking search!');
            return;
        }
        this.staffAttendsRptSvc
            .getStaffAttendancesReport(
                saSearch.month,
                saSearch.year,
                saSearch.staffCategoryId,
                saSearch.status
            )
            .subscribe({
                next: (staffAttendsRpt) => {
                    this.currentRptMonth = saSearch.month;
                    this.currentRptYear = saSearch.year.toString();
                    this.currentRptStatus = saSearch.status;
                    this.currentRptStaffCategory = this.staffCategories.find(
                        (s) => s.id == saSearch.staffCategoryId.toString()
                    ).name;
                    this.staffAttendancesRpt = staffAttendsRpt;
                    this.syncUrl({
                        staffCategoryId: saSearch.staffCategoryId,
                        status: saSearch.status,
                        month: saSearch.month,
                        year: saSearch.year
                    });
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    printIndividualReport = (staffId: number) => {
        let schDetailsReq = this.schoolSvc.get('/schooldetails');
        let staffAttendReq =
            this.staffAttendancesSvc.searchStaffAttendancesObservable(
                staffId,
                this.currentRptMonth,
                parseInt(this.currentRptYear)
            );

        forkJoin([schDetailsReq, staffAttendReq]).subscribe({
            next: ([school, attends]) => {
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
                const staffAttends = [];
                staffAttends.push(attends);

                this.saRDSvc.printByBatch(staffAttends, school[0], reportTitle);
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
                    this.currentRptStaffCategory.toLocaleUpperCase() +
                    ' STAFF ATTENDANCE REPORT FOR ' +
                    new Date(0, this.currentRptMonth - 1)
                        .toLocaleString('default', {month: 'long'})
                        .toUpperCase() +
                    ' ' +
                    this.currentRptYear.toLocaleUpperCase();
                this.staffAttendsRptSvc.generateReport(
                    school[0],
                    this.staffAttendancesRpt,
                    reportTitle
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };
}
