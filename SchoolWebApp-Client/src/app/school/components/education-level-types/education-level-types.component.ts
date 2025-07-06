import {BreadCrumb} from '@/core/models/bread-crumb';
import {EducationLevelType} from '@/school/models/education-level-types';
import {EducationLevelTypesService} from '@/school/services/education-level-types.service';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import { forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-education-level-types',
    templateUrl: './education-level-types.component.html',
    styleUrl: './education-level-types.component.scss'
})
export class EducationLevelTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/settings/educationLevelTypes'],
            title: 'Settings: Education level types'
        }
    ];

    dashboardTitle = 'Settings:  Education level types';
    tableTitle: string = ' Education level types list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Abbreviation', 'Description', 'Action'];

    educationLevelType: EducationLevelType;
    educationLevelTypes: EducationLevelType[] = [];
    tblShowViewButton: true;
    editMode = false;
    isAuthLoading: boolean;
    page = 1;
    pageSize = 10;
    tableModel: string = 'educationLevelType';

    educationLevelTypeForm: FormGroup;

    constructor(
        private educationLevelTypesSvc: EducationLevelTypesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };


    get f() {
        return this.educationLevelTypeForm.controls;
    }

    refreshItems() {
        this.educationLevelTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbr: [''],
            description: ['']
        });

        let educationLevelTypesRequest = this.educationLevelTypesSvc.get(
            '/educationLevelTypes'
        );

        forkJoin([educationLevelTypesRequest]).subscribe(
            (res) => {
                this.educationLevelTypes = res[0];
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.educationLevelTypesSvc
            .getById(id, '/educationLevelTypes')
            .subscribe(
                (res) => {
                    this.educationLevelType = new EducationLevelType(res);
                    this.educationLevelTypeForm.setValue({
                        name: this.educationLevelType.name,
                        abbr: this.educationLevelType.abbr,
                        description: this.educationLevelType.description
                    });
                    this.editMode = true;
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
                this.educationLevelTypesSvc
                    .delete('/educationLevelTypes', id)
                    .subscribe(
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

    onSubmit() {
        if (this.educationLevelTypeForm.invalid) {
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${
                this.editMode ? 'edit' : 'add'
            } record.`,
            width: 400,
            heightAuto: true,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                if (this.editMode) {
                    this.educationLevelType.name =
                        this.educationLevelTypeForm.get('name').value;
                    this.educationLevelType.abbr =
                        this.educationLevelTypeForm.get('abbr').value;
                    this.educationLevelType.description =
                        this.educationLevelTypeForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.educationLevelTypesSvc.update(
                          '/educationLevelTypes',
                          this.educationLevelType
                      )
                    : this.educationLevelTypesSvc.create(
                          '/educationLevelTypes',
                          new EducationLevelType(
                              this.educationLevelTypeForm.value
                          )
                      );

                let replyMsg = `Education level type ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.educationLevelTypeForm.reset();
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

    resetForm() {
        this.educationLevelTypeForm.reset();
    }
}
