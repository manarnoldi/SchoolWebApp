import {BreadCrumb} from '@/core/models/bread-crumb';
import {LearningMode} from '@/school/models/learning-mode';
import {LearningModesService} from '@/school/services/learning-modes.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-learning-modes',
    templateUrl: './learning-modes.component.html',
    styleUrl: './learning-modes.component.scss'
})
export class LearningModesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    learningModeForm: FormGroup;

    buttonTitle: string = 'Add learning mode';
    tableModel: string = 'learningMode';
    tableTitle: string = 'Learning modes list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    learningMode: LearningMode;
    isAuthLoading: boolean;
    learningModes: LearningMode[] = [];
    tblShowViewButton: false;

    constructor(
        private learningModesSvc: LearningModesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Learning modes list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/learningModes'], title: 'Settings:Learning modes'}
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
                this.learningModesSvc.delete('/learningModes', id).subscribe(
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
        this.learningModesSvc.getById(id, '/learningModes').subscribe(
            (res) => {
                this.learningMode = new LearningMode(res);
                this.learningModeForm.setValue({
                    name: this.learningMode.name,
                    rank: this.learningMode.rank,
                    description: this.learningMode.description
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
        if (this.learningModeForm.invalid) {
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
                    this.learningMode.name =
                        this.learningModeForm.get('name').value;
                    this.learningMode.rank =
                        this.learningModeForm.get('rank').value;
                    this.learningMode.description =
                        this.learningModeForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.learningModesSvc.update(
                          '/learningModes',
                          this.learningMode
                      )
                    : this.learningModesSvc.create(
                          '/learningModes',
                          new LearningMode(this.learningModeForm.value)
                      );

                let replyMsg = `Learning mode ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.learningModeForm.reset();
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
        return this.learningModeForm.controls;
    }

    refreshItems() {
        this.learningModeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.learningModesSvc.get('/learningModes').subscribe(
            (res) => {
                this.learningModes = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the learning modes. Contact system administrator.'
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
        this.learningModeForm.reset();
        this.editMode = false;
    }
}
