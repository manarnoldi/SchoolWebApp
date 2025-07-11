import {BreadCrumb} from '@/core/models/bread-crumb';
import {Designation} from '@/settings/models/designation';
import {DesignationsService} from '@/settings/services/designations.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-designations',
    templateUrl: './designations.component.html',
    styleUrl: './designations.component.scss'
})
export class DesignationsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    designationForm: FormGroup;

    buttonTitle: string = 'Add designation';
    tableModel: string = 'designation';
    tableTitle: string = 'Designations list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    designation: Designation;
    isAuthLoading: boolean;
    designations: Designation[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private designationsSvc: DesignationsService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Designations list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/designations'], title: 'Settings:Designations'}
    ];

    deleteItem(id) {
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
                this.designationsSvc.delete('/designations', id).subscribe(
                    (res) => {
                        this.refreshItems();
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

    editItem(id) {
        this.designationsSvc.getById(id, '/designations').subscribe(
            (res) => {
                this.designation = new Designation(res);
                this.designationForm.setValue({
                    name: this.designation.name,
                    rank: this.designation.rank,
                    description: this.designation.description
                });
                this.editMode = true;
                this.settingsTblBtn.onButtonClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    onSubmit() {
        if (this.designationForm.invalid) {
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${
                this.editMode ? 'edit' : 'add'
            } record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                if (this.editMode) {
                    this.designation.name =
                        this.designationForm.get('name').value;
                    this.designation.description =
                        this.designationForm.get('description').value;
                    this.designation.rank =
                        this.designationForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.designationsSvc.update(
                          '/designations',
                          this.designation
                      )
                    : this.designationsSvc.create(
                          '/designations',
                          new Designation(this.designationForm.value)
                      );

                let replyMsg = `Designation ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.designationForm.reset();
                        this.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    get f() {
        return this.designationForm.controls;
    }

    refreshItems() {
        this.designationForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0],
            description: ['']
        });

        this.designationsSvc.get('/designations').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.designations = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.designations.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the designations. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }
    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    resetForm() {
        this.designationForm.reset();
        this.editMode = false;
    }
}
