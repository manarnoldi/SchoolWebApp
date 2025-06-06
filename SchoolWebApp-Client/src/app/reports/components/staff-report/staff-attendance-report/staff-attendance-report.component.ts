import {StaffAttendancesReport} from '@/reports/models/staff-attendances-report';
import {StaffAttendancesReportService} from '@/reports/services/staff-attendances-report.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EmploymentType} from '@/settings/models/employment-type';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {DateMonthYear} from '@/shared/models/date-month-year';
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
    employmentTypes: EmploymentType[] = [];

    months: number[];
    years: number[];

    constructor(
        private toastr: ToastrService,
        private staffCategoriesSvc: StaffCategoriesService,
        private acadYearsSvc: AcademicYearsService,
        private staffAttendsRptSvc: StaffAttendancesReportService
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
        this.staffAttendancesRpt = [];
    };

    monthChangedChanged = (employTypeId: number) => {
        this.staffAttendancesRpt = [];
    };
    staffCategoryChanged = (sCategoryId: number) => {
        this.staffAttendancesRpt = [];
    };

    searchForDataMethod = (saSearch: DateMonthYear) => {
        this.staffAttendancesRpt = [];
        if (!saSearch.staffCategoryId || saSearch.staffCategoryId == null) {
            this.toastr.error('Select staff category before clicking search!');
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
                saSearch.staffCategoryId
            )
            .subscribe({
                next: (staffAttendsRpt) => {
                    this.staffAttendancesRpt = staffAttendsRpt;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };
}
