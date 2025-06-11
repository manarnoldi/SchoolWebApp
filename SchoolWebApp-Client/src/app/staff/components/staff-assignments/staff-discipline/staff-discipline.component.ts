import {StaffDetails} from '@/staff/models/staff-details';
import {
    AfterViewInit,
    Component,
    Input,
    OnInit,
    ViewChild
} from '@angular/core';
import {StaffDisciplineFormComponent} from './staff-discipline-form/staff-discipline-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StaffDiscipline} from '@/staff/models/staff-discipline';
import {ToastrService} from 'ngx-toastr';
import {StaffDisciplinesService} from '@/staff/services/staff-disciplines.service';
import {ActivatedRoute} from '@angular/router';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';
import {DateMonthYear} from '@/shared/models/date-month-year';
import {DatePipe} from '@angular/common';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-staff-discipline',
    templateUrl: './staff-discipline.component.html',
    styleUrl: './staff-discipline.component.scss'
})
export class StaffDisciplineComponent implements OnInit, AfterViewInit {
    @Input() statuses;
    @Input() staff: StaffDetails;
    @Input() outcomes: Outcome[];
    @Input() occurenceTypes: OccurenceType[];
    @ViewChild(StaffDisciplineFormComponent)
    staffDisciplineFormComponent: StaffDisciplineFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    firstLoad: boolean = true;
    doneLoading: boolean = false;
    staffId: number = 0;
    staffDiscipline: StaffDiscipline;
    staffDisciplines: StaffDiscipline[] = [];

    constructor(
        private toastr: ToastrService,
        private staffDisciplinesSvc: StaffDisciplinesService,
        private route: ActivatedRoute,
        private datePipe: DatePipe
    ) {}
    ngOnInit(): void {}

    ngAfterViewInit(): void {
        this.loadStaffDisciplines();
    }

    dateFromChanged = (id: number) => {
        this.staffDisciplines = [];
    };

    dateToChanged = (id: number) => {
        this.staffDisciplines = [];
    };

    searchByDate = (dmy: DateMonthYear) => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            this.staffDisciplinesSvc
                .getByDateFromDateToStaffId(
                    this.staffId,
                    this.datePipe.transform(dmy.dateFrom, 'yyyy-MM-dd'),
                    this.datePipe.transform(dmy.dateTo, 'yyyy-MM-dd')
                )
                .subscribe({
                    next: (staffDisciplines) => {
                        this.staffDisciplines = staffDisciplines.sort(
                            (a, b) =>
                                new Date(
                                    a?.occurenceStartDate ?? ''
                                ).getTime() -
                                new Date(b?.occurenceStartDate ?? '').getTime()
                        );
                        if (this.firstLoad) {
                            this.ssFilterFormComponent.setFormControls(dmy);
                        }
                        if (
                            this.staffDisciplines.length <= 0 &&
                            !this.firstLoad
                        ) {
                            this.toastr.info(
                                'No staff discipline record/s found for the search parameters!'
                            );
                        }
                        this.firstLoad = false;
                        this.doneLoading = true;
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        });
    };

    loadStaffDisciplines = () => {
        const today = new Date();
        const dateFrom = new Date(today.getFullYear(), 0, 1);
        const dateTo = new Date(today.getFullYear(), 11, 31);

        let dmy = new DateMonthYear();
        dmy.dateFrom = dateFrom;
        dmy.dateTo = dateTo;

        this.searchByDate(dmy);
    };

    editItem(id: number, action = 'edit') {
        this.staffDisciplinesSvc.getById(id, '/staffDisciplines').subscribe(
            (res) => {
                let staffDisciplineId = res.id;
                this.staffDiscipline = new StaffDiscipline(res);
                this.staffDiscipline.id = staffDisciplineId;
                this.staffDisciplineFormComponent.setFormControls(
                    this.staffDiscipline
                );
                this.staffDisciplineFormComponent.action = action;
                this.staffDisciplineFormComponent.staffDiscipline =
                    this.staffDiscipline;
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
                this.staffDisciplinesSvc
                    .delete('/staffDisciplines', id)
                    .subscribe(
                        (res) => {
                            this.loadStaffDisciplines();
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

    AddStaffDiscipline = (staffDiscipline: StaffDiscipline) => {
        Swal.fire({
            title: `${this.staffDisciplineFormComponent.action == 'edit' ? 'Update' : 'Add'} Staff discipline record?`,
            text: `Confirm if you want to ${
                this.staffDisciplineFormComponent.action == 'edit'
                    ? 'update'
                    : 'add'
            } staff discipline.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.staffDisciplineFormComponent.action == 'edit' ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new StaffDiscipline(staffDiscipline);
                if (this.staffDisciplineFormComponent.action == 'edit')
                    app.id = staffDiscipline.id;
                let reqToProcess =
                    this.staffDisciplineFormComponent.action == 'edit'
                        ? this.staffDisciplinesSvc.update(
                              '/staffDisciplines',
                              app
                          )
                        : this.staffDisciplinesSvc.create(
                              '/staffDisciplines',
                              app
                          );

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.staffDisciplineFormComponent.action = 'add';
                        this.toastr.success(
                            'Staff discipline saved successfully'
                        );
                        this.staffDisciplineFormComponent.closeButton.nativeElement.click();
                        this.loadStaffDisciplines();
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
