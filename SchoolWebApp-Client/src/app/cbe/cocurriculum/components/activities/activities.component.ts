import {BreadCrumb} from '@/core/models/bread-crumb';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {CoCurriculumActivity} from '../../models/co-curriculum-activity';
import {CoCurriculumActivityService} from '../../services/co-curriculum-activity.service';

@Component({
    selector: 'app-activities',
    templateUrl: './activities.component.html',
    styleUrl: './activities.component.scss'
})
export class ActivitiesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    activityForm: FormGroup;

    buttonTitle: string = 'Add activity';
    tableModel: string = 'coCurriculumActivity';
    tableTitle: string = 'Co-curriculum activities list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    coCurriculumActivity: CoCurriculumActivity;
    isAuthLoading: boolean;
    coCurriculumActivities: CoCurriculumActivity[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private coCurriculumActivitySvc: CoCurriculumActivityService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Co-curricular: Activities Register';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/cocurriculum/activities'], title: 'CBE Co-curricular: Activities Register'}
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
                this.coCurriculumActivitySvc.delete('/coCurriculumActivities', id).subscribe(
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
        this.coCurriculumActivitySvc.getById(id, '/coCurriculumActivities').subscribe(
            (res) => {
                this.coCurriculumActivity = new CoCurriculumActivity(res);
                this.activityForm.setValue({
                    name: this.coCurriculumActivity.name,
                    rank: this.coCurriculumActivity.rank,
                    description: this.coCurriculumActivity.description
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
        if (this.activityForm.invalid) {
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${this.editMode ? 'edit' : 'add'} record.`,
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
                    this.coCurriculumActivity.name = this.activityForm.get('name').value;
                    this.coCurriculumActivity.description = this.activityForm.get('description').value;
                    this.coCurriculumActivity.rank = this.activityForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.coCurriculumActivitySvc.update('/coCurriculumActivities', this.coCurriculumActivity)
                    : this.coCurriculumActivitySvc.create('/coCurriculumActivities', new CoCurriculumActivity(this.activityForm.value));

                let replyMsg = `Co-curriculum activity ${this.editMode ? 'updated' : 'created'} successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.activityForm.reset();
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
        return this.activityForm.controls;
    }

    refreshItems() {
        this.activityForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.coCurriculumActivitySvc.get('/coCurriculumActivities').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.coCurriculumActivities = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.coCurriculumActivities.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the co-curriculum activities. Contact system administrator.'
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
        this.activityForm.reset();
        this.editMode = false;
    }
}
