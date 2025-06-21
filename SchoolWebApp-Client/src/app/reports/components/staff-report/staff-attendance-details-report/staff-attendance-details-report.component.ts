import {Status} from '@/core/enums/status';
import {StaffAttendancesReportDetailsService} from '@/reports/services/staff-attendances-report-details.service';
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
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, Observable} from 'rxjs';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

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

    constructor(
        private toastr: ToastrService,
        private staffDetailsSvc: StaffDetailsService,
        private acadYearsSvc: AcademicYearsService,
        private staffAttendancesRptDetailsSvc: StaffAttendancesReportDetailsService,
        private staffAttendancesSvc: StaffAttendancesService,
        private staffCategoriesSvc: StaffCategoriesService,
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

    yearChanged = (selectedDate: Date) => {
        this.staffAttendances = [];
        this.staffDetails = [];
    };

    monthChanged = (month: number) => {
        this.staffAttendances = [];
        this.staffDetails = [];
        this.currentRptMonth = month;
    };

    statusChanged = (status: Status) => {
        this.staffAttendances = [];
        this.staffDetails = [];
    };

    staffCategoryChanged = (sCategoryId: number) => {
        this.staffAttendances = [];
        this.staffDetails = [];
    };

    staffClicked = (staffId: number) => {
        this.staffAttendances = [];
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

    searchForDataMethod = (saSearch: SchoolSoftFilter) => {
        this.staffAttendances = [];
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

        this.currentRptMonth = this.months.find(
            (m) => m == this.currentRptMonth
        );
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
