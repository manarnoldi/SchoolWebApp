import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {
    AfterViewInit,
    Component,
    Input,
    OnInit,
    ViewChild
} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StaffAttendanceFormComponent} from './staff-attendance-form/staff-attendance-form.component';
import {StaffDetails} from '@/staff/models/staff-details';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';

@Component({
    selector: 'app-staff-attendance',
    templateUrl: './staff-attendance.component.html',
    styleUrl: './staff-attendance.component.scss'
})
export class StaffAttendanceComponent implements OnInit, AfterViewInit {
    @Input() statuses;
    @Input() staff: StaffDetails;
    @ViewChild(StaffAttendanceFormComponent)
    staffAttendanceFormComponent: StaffAttendanceFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    isDoneLoading = false;

    today = new Date();
    firstLoad: boolean = true;
    staffId: number = 0;
    months: number[];
    years: number[];

    staffAttendance: StaffAttendance;
    staffAttendances: StaffAttendance[] = [];
    constructor(
        private toastr: ToastrService,
        private staffAttendancesSvc: StaffAttendancesService,
        private academicYearsSvc: AcademicYearsService,
        private route: ActivatedRoute
    ) {}

    ngAfterViewInit(): void {
        this.loadStaffAttendances();
    }

    ngOnInit(): void {}

    searchStaffAttendances = (dmy: SchoolSoftFilter) => {
        if (!dmy.month || dmy.month < 1 || dmy.month > 12) {
            this.toastr.error('The month selected is not valid/correct!');
            return;
        } else if (!dmy.year) {
            this.toastr.error('The year selected is not valid/correct!');
            return;
        }
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            this.staffAttendancesSvc
                .getByMonthYearStaffId(dmy.month, dmy.year, this.staffId)
                .subscribe({
                    next: (staffAttends) => {
                        this.staffAttendances = staffAttends.sort(
                            (a, b) =>
                                new Date(a?.date ?? '').getTime() -
                                new Date(b?.date ?? '').getTime()
                        );
                        if (this.firstLoad) {
                            this.ssFilterFormComponent.setFormControls(dmy);
                        }
                        if (
                            this.staffAttendances.length <= 0 &&
                            !this.firstLoad
                        ) {
                            this.toastr.info(
                                'No staff attendance record/s found for the search parameters!'
                            );
                        }
                        this.firstLoad = false;
                        this.isDoneLoading = true;
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        });
    };

    monthChanged = (month: number) => {
        this.staffAttendances = [];
    };

    yearChanged = (year: number) => {
        this.staffAttendances = [];
    };

    loadStaffAttendances = () => {
        let yearsReq = this.academicYearsSvc.get('/academicYears');
        forkJoin([yearsReq]).subscribe({
            next: ([years]) => {
                this.years = [
                    ...new Set(
                        years.map((yr) => new Date(yr.startDate).getFullYear())
                    )
                ].sort((a, b) => b - a);
                this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
                const topMonth = this.today.getMonth() + 1;
                const topYear = this.years[0];

                let dmy = new SchoolSoftFilter();
                dmy.month = topMonth;
                dmy.year = topYear;

                this.searchStaffAttendances(dmy);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    editItem(id: number) {
        this.staffAttendancesSvc.getById(id, '/staffAttendances').subscribe(
            (res) => {
                let staffAttendanceId = res.id;
                this.staffAttendance = new StaffAttendance(res);
                this.staffAttendance.id = staffAttendanceId;
                this.staffAttendanceFormComponent.setFormControls(
                    this.staffAttendance
                );
                this.staffAttendanceFormComponent.editMode = true;
                this.staffAttendanceFormComponent.staffAttendance =
                    this.staffAttendance;
                this.tableButton.onClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.staffAttendancesSvc
                    .delete('/staffAttendances', id)
                    .subscribe(
                        (res) => {
                            const topMonth =
                                this.ssFilterFormComponent.schoolSoftFilterForm.get(
                                    'month'
                                ).value;
                            const topYear =
                                this.ssFilterFormComponent.schoolSoftFilterForm.get(
                                    'year'
                                ).value;
                            let dmy = new SchoolSoftFilter();
                            dmy.month = topMonth;
                            dmy.year = topYear;
                            this.searchStaffAttendances(dmy);
                            this.toastr.success('Record deleted successfully!');
                        },
                        (err) => {
                            this.toastr.error(err);
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    AddStaffAttendance = (staffAttendance: StaffAttendance) => {
        Swal.fire({
            title: `${this.staffAttendanceFormComponent.editMode ? 'Update' : 'Add'} Staff attendance record?`,
            text: `Confirm if you want to ${
                this.staffAttendanceFormComponent.editMode ? 'update' : 'add'
            } staff attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.staffAttendanceFormComponent.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StaffAttendance(staffAttendance);
                if (this.staffAttendanceFormComponent.editMode)
                    app.id = staffAttendance.id;
                let reqToProcess = this.staffAttendanceFormComponent.editMode
                    ? this.staffAttendancesSvc.update('/staffAttendances', app)
                    : this.staffAttendancesSvc.create('/staffAttendances', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.staffAttendanceFormComponent.editMode = false;
                        this.toastr.success(
                            'Staff attendance saved successfully'
                        );
                        this.staffAttendanceFormComponent.closeButton.nativeElement.click();

                        const topMonth =
                            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                                'month'
                            ).value;
                        const topYear =
                            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                                'year'
                            ).value;
                        let dmy = new SchoolSoftFilter();
                        dmy.month = topMonth;
                        dmy.year = topYear;
                        this.searchStaffAttendances(dmy);
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
