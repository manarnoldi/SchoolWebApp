import {BreadCrumb} from '@/core/models/bread-crumb';
import {EmploymentType} from '@/settings/models/employment-type';
import {EmploymentTypeService} from '@/settings/services/employment-type.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-employment-types',
    templateUrl: './employment-types.component.html',
    styleUrl: './employment-types.component.scss'
})
export class EmploymentTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    employmentTypeForm: FormGroup;

    buttonTitle: string = 'Add employment type';
    tableModel: string = 'employmentType';
    tableTitle: string = 'Employment Types list';
    tableHeaders: string[] = ['Name', 'Description', 'Action'];

    editMode = false;
    employmentType: EmploymentType;
    isAuthLoading: boolean;
    employmentTypes: EmploymentType[] = [];
    tblShowViewButton: false;

    constructor(
        private employmentTypeSvc: EmploymentTypeService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Employment types list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/settings/employmentTypes'],
            title: 'Settings:Employment types'
        }
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
                this.employmentTypeSvc.delete('/employmentTypes', id).subscribe(
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
        this.employmentTypeSvc.getById(id, '/employmentTypes').subscribe(
            (res) => {
                this.employmentType = new EmploymentType(res);
                this.employmentTypeForm.setValue({
                    name: this.employmentType.name,
                    description: this.employmentType.description
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
        if (this.employmentTypeForm.invalid) {
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
                    this.employmentType.name =
                        this.employmentTypeForm.get('name').value;
                    this.employmentType.description =
                        this.employmentTypeForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.employmentTypeSvc.update(
                          '/employmentTypes',
                          this.employmentType
                      )
                    : this.employmentTypeSvc.create(
                          '/employmentTypes',
                          new EmploymentType(this.employmentTypeForm.value)
                      );

                let replyMsg = `EmploymentType ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.employmentTypeForm.reset();
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
        return this.employmentTypeForm.controls;
    }

    refreshItems() {
        this.employmentTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: ['']
        });

        this.employmentTypeSvc.get('/employmentTypes').subscribe(
            (res) => {
                this.employmentTypes = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the employment types. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.refreshItems();
    }

    resetForm() {
        this.employmentTypeForm.reset();
        this.editMode = false;
    }
}
