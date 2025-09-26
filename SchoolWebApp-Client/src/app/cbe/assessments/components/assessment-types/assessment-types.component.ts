import { SettingsTableComponent } from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AssessmentType } from '../../models/assessment-type';
import { AssessmentTypeService } from '../../services/assessment-type.service';
import { ToastrService } from 'ngx-toastr';
import { BreadCrumb } from '@/core/models/bread-crumb';
import Swal from 'sweetalert2';
import { forkJoin } from 'rxjs';

@Component({
    selector: 'app-assessment-types',
    templateUrl: './assessment-types.component.html',
    styleUrl: './assessment-types.component.scss'
})
export class AssessmentTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    assessmentTypeForm: FormGroup;

    buttonTitle: string = 'Add assessment type';
    tableModel: string = 'assessmentType';
    tableTitle: string = 'Assessment types list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    assessmentType: AssessmentType;
    isAuthLoading: boolean;
    assessmentTypes: AssessmentType[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private assessmentTypesSvc: AssessmentTypeService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Assessment types list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/assessments/assessment-types'], title: 'CBE-Assessments: Assessment Types'}
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
                this.assessmentTypesSvc.delete('/assessmentTypes', id).subscribe(
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
        this.assessmentTypesSvc.getById(id, '/assessmentTypes').subscribe(
            (res) => {
                this.assessmentType = new AssessmentType(res);
                this.assessmentTypeForm.setValue({
                    name: this.assessmentType.name,
                    rank: this.assessmentType.rank,
                    description: this.assessmentType.description
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
        if (this.assessmentTypeForm.invalid) {
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
                    this.assessmentType.name =
                        this.assessmentTypeForm.get('name').value;
                    this.assessmentType.description =
                        this.assessmentTypeForm.get('description').value;
                    this.assessmentType.rank =
                        this.assessmentTypeForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.assessmentTypesSvc.update(
                          '/assessmentTypes',
                          this.assessmentType
                      )
                    : this.assessmentTypesSvc.create(
                          '/assessmentTypes',
                          new AssessmentType(this.assessmentTypeForm.value)
                      );

                let replyMsg = `AssessmentType ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.assessmentTypeForm.reset();
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
        return this.assessmentTypeForm.controls;
    }

    refreshItems() {
        this.assessmentTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.assessmentTypesSvc.get('/assessmentTypes').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.assessmentTypes = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.assessmentTypes.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the assessmentTypes. Contact system administrator.'
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
        this.assessmentTypeForm.reset();
        this.editMode = false;
    }
}
