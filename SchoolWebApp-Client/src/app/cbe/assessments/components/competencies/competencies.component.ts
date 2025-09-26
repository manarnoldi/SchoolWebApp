import { BreadCrumb } from '@/core/models/bread-crumb';
import { SettingsTableComponent } from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { forkJoin } from 'rxjs';
import Swal from 'sweetalert2';
import { Competency } from '../../models/competency';
import { CompetencyService } from '../../services/competency.service';

@Component({
    selector: 'app-competencies',
    templateUrl: './competencies.component.html',
    styleUrl: './competencies.component.scss'
})
export class CompetenciesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    competencyForm: FormGroup;

    buttonTitle: string = 'Add competency type';
    tableModel: string = 'competency';
    tableTitle: string = 'Competency types list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    competency: Competency;
    isAuthLoading: boolean;
    competencies: Competency[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private competenciesSvc: CompetencyService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Competency types list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/cbe/assessments/competency-types'],
            title: 'CBE-Competencys: Competencies'
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
                this.competenciesSvc
                    .delete('/competencies', id)
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

    editItem(id) {
        this.competenciesSvc.getById(id, '/competencies').subscribe(
            (res) => {
                this.competency = new Competency(res);
                this.competencyForm.setValue({
                    name: this.competency.name,
                    rank: this.competency.rank,
                    description: this.competency.description
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
        if (this.competencyForm.invalid) {
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
                    this.competency.name =
                        this.competencyForm.get('name').value;
                    this.competency.description =
                        this.competencyForm.get('description').value;
                    this.competency.rank =
                        this.competencyForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.competenciesSvc.update(
                          '/competencies',
                          this.competency
                      )
                    : this.competenciesSvc.create(
                          '/competencies',
                          new Competency(this.competencyForm.value)
                      );

                let replyMsg = `Competency ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.competencyForm.reset();
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
        return this.competencyForm.controls;
    }

    refreshItems() {
        this.competencyForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.competenciesSvc.get('/competencies').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.competencies = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.competencies.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the competencies. Contact system administrator.'
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
        this.competencyForm.reset();
        this.editMode = false;
    }
}
