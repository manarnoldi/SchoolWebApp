import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StaffAttendanceFormComponent} from './staff-attendance-form/staff-attendance-form.component';
import {StaffDetails} from '@/staff/models/staff-details';
import { TableButtonComponent } from '@/shared/directives/table-button/table-button.component';

@Component({
    selector: 'app-staff-attendance',
    templateUrl: './staff-attendance.component.html',
    styleUrl: './staff-attendance.component.scss'
})
export class StaffAttendanceComponent implements OnInit {
    @Input() statuses;
    @Input() staff: StaffDetails;
    @ViewChild(StaffAttendanceFormComponent)
    staffAttendanceFormComponent: StaffAttendanceFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    staffId: number = 0;
    staffAttendance: StaffAttendance;
    staffAttendances: StaffAttendance[] = [];
    constructor(
        private toastr: ToastrService,
        private staffAttendancesSvc: StaffAttendancesService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStaffAttendances();
    }

    loadStaffAttendances = () => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            let attendanceByStaffDetailsIdReq = this.staffAttendancesSvc.get(
                '/staffAttendances/byStaffDetailsId/' + this.staffId.toString()
            );

            forkJoin([attendanceByStaffDetailsIdReq]).subscribe(
                ([staffAttendances]) => {
                    this.staffAttendances = staffAttendances;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };

    editItem(id: number) {
        this.staffAttendancesSvc.getById(id, '/staffAttendances').subscribe(
            (res) => {
                let staffAttendanceId = res.id;
                this.staffAttendance = new StaffAttendance(res);
                this.staffAttendance.id = staffAttendanceId;
                this.staffAttendanceFormComponent.setFormControls(this.staffAttendance);
                this.staffAttendanceFormComponent.editMode = true;
                this.staffAttendanceFormComponent.staffAttendance = this.staffAttendance;
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
                this.staffAttendancesSvc.delete('/staffAttendances', id).subscribe(
                    (res) => {
                        this.loadStaffAttendances();
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
            } education Level.`,
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
                        this.loadStaffAttendances();
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
