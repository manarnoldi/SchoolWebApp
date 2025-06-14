import {Status} from '@/core/enums/status';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {DateMonthYear} from '@/shared/models/date-month-year';
import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

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

    constructor(
        private toastr: ToastrService,
        private staffDetailsSvc: StaffDetailsService,
        private acadYearsSvc: AcademicYearsService,
        private staffAttendancesSvc: StaffAttendancesService,
        private staffCategoriesSvc: StaffCategoriesService
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
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    yearChanged = (selectedDate: Date) => {
        this.staffAttendances = [];
    };

    monthChangedChanged = (employTypeId: number) => {
        this.staffAttendances = [];
    };

    statusChanged = (status: Status) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.statusId = status;
        this.ssFilterFormComponent.schoolSoftFilterForm
            .get('staffDetailsId')
            .reset();

        this.staffDetailsSvc
            .getBySearchDetails(this.statusId, null, this.sCategoryId)
            .subscribe({
                next: (staffDetails) => {
                    this.staffDetails = staffDetails;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    staffCategoryChanged = (sCategoryId: number) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.sCategoryId = sCategoryId;
        this.ssFilterFormComponent.schoolSoftFilterForm
            .get('staffDetailsId')
            .reset();

        this.staffDetailsSvc
            .getBySearchDetails(this.statusId, null, this.sCategoryId)
            .subscribe({
                next: (staffDetails) => {
                    this.staffDetails = staffDetails;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    staffDetailsChanged = (staffId: number) => {
        this.staffAttendances = [];
    };

    searchForDataMethod = (saSearch: DateMonthYear) => {
        this.staffAttendances = [];
        if (!saSearch.staffCategoryId || saSearch.staffCategoryId == null) {
            this.toastr.error('Select staff category before clicking search!');
            return;
        } else if (
            !saSearch.staffDetailsId ||
            !saSearch.staffDetailsId == null
        ) {
            this.toastr.error('Select staff first before clicking search!');
            return;
        } else if (!saSearch.month || saSearch.month == null) {
            this.toastr.error('Select month before clicking search!');
            return;
        } else if (!saSearch.year || saSearch.year == null) {
            this.toastr.error('Select year before clicking search!');
            return;
        }
        this.staffAttendancesSvc
            .getByMonthYearStaffId(
                saSearch.month,
                saSearch.year,
                saSearch.staffDetailsId
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
}
