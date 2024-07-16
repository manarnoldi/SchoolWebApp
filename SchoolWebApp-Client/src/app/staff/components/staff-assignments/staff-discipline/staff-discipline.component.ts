import {StaffDetails} from '@/staff/models/staff-details';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {StaffDisciplineFormComponent} from './staff-discipline-form/staff-discipline-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {StaffDiscipline} from '@/staff/models/staff-discipline';
import {ToastrService} from 'ngx-toastr';
import {StaffDisciplinesService} from '@/staff/services/staff-disciplines.service';
import {ActivatedRoute} from '@angular/router';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {OutcomesService} from '@/settings/services/outcomes.service';
import {OccurenceTypeService} from '@/settings/services/occurence-type.service';
import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';

@Component({
    selector: 'app-staff-discipline',
    templateUrl: './staff-discipline.component.html',
    styleUrl: './staff-discipline.component.scss'
})
export class StaffDisciplineComponent implements OnInit {
    @Input() statuses;
    @Input() staff: StaffDetails;
    @Input() outcomes: Outcome[];
    @Input() occurenceTypes: OccurenceType[];
    @ViewChild(StaffDisciplineFormComponent)
    staffDisciplineFormComponent: StaffDisciplineFormComponent;
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    staffId: number = 0;
    staffDiscipline: StaffDiscipline;
    staffDisciplines: StaffDiscipline[] = [];
   
    constructor(
        private toastr: ToastrService,
        private staffDisciplinesSvc: StaffDisciplinesService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.loadStaffDisciplines();
    }

    loadStaffDisciplines = () => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            let disciplineByStaffDetailsIdReq = this.staffDisciplinesSvc.get(
                '/staffDisciplines/byStaffDetailsId/' + this.staffId.toString()
            );            

            forkJoin([
                disciplineByStaffDetailsIdReq,
            ]).subscribe(
                ([staffDisciplines]) => {
                    this.staffDisciplines = staffDisciplines;                    
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
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
                let reqToProcess = this.staffDisciplineFormComponent.action == 'edit'
                    ? this.staffDisciplinesSvc.update('/staffDisciplines', app)
                    : this.staffDisciplinesSvc.create('/staffDisciplines', app);

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
