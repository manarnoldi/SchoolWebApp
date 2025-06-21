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
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

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
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    academicYearChanged = (academicYearChanged: number) => {
        this.currentRptYearId = academicYearChanged;
        this.staffSubjects = [];
        this.staffDetails = [];
    };

    statusChanged = (status: Status) => {
        this.staffSubjects = [];
        this.staffDetails = [];
    };

    staffCategoryChanged = (sCategoryId: number) => {
        this.staffSubjects = [];
        this.staffDetails = [];
    };

    staffClicked = (staffId: number) => {
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

    searchForDataMethod = (saSearch: SchoolSoftFilter) => {
        this.staffSubjects = [];
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
