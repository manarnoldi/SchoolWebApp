import {Status} from '@/core/enums/status';
import {StaffAttendancesReport} from '@/reports/models/staff-attendances-report';
import {StaffAttendancesReportService} from '@/reports/services/staff-attendances-report.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-staff-attendance-report',
    templateUrl: './staff-attendance-report.component.html',
    styleUrl: './staff-attendance-report.component.scss'
})
export class StaffAttendanceReportComponent implements OnInit {
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
        private schoolSvc: SchoolDetailsService
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

    statusChanged = (status: Status) => {
        this.resetControlls();
    };

    yearChanged = (selectedDate: Date) => {
        this.resetControlls();
    };

    monthChangedChanged = (employTypeId: number) => {
        this.resetControlls();
    };
    staffCategoryChanged = (sCategoryId: number) => {
        this.resetControlls();
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
        } else if (
            saSearch.status == null
        ) {
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
                    Status[this.currentRptStatus].toUpperCase() + ' ' +
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
