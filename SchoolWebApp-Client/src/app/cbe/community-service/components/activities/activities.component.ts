import {BreadCrumb} from '@/core/models/bread-crumb';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {CommunityServiceActivity} from '../../models/community-service-activity';
import {CommunityServiceActivityService} from '../../services/community-service-activity.service';

@Component({
    selector: 'app-community-service-activities',
    templateUrl: './activities.component.html',
    styleUrl: './activities.component.scss'
})
export class CommunityServiceActivitiesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    activityForm: FormGroup;

    buttonTitle: string = 'Add activity';
    tableModel: string = 'communityServiceActivity';
    tableTitle: string = 'Community service activities list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    communityServiceActivity: CommunityServiceActivity;
    isAuthLoading: boolean;
    communityServiceActivities: CommunityServiceActivity[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private communityServiceActivitySvc: CommunityServiceActivityService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Community Service: Activities Register';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/community-service/activities'], title: 'CBE Community Service: Activities Register'}
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
                this.communityServiceActivitySvc.delete('/communityServiceActivities', id).subscribe(
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
        this.communityServiceActivitySvc.getById(id, '/communityServiceActivities').subscribe(
            (res) => {
                this.communityServiceActivity = new CommunityServiceActivity(res);
                this.activityForm.setValue({
                    name: this.communityServiceActivity.name,
                    rank: this.communityServiceActivity.rank,
                    description: this.communityServiceActivity.description
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
                    this.communityServiceActivity.name = this.activityForm.get('name').value;
                    this.communityServiceActivity.description = this.activityForm.get('description').value;
                    this.communityServiceActivity.rank = this.activityForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.communityServiceActivitySvc.update('/communityServiceActivities', this.communityServiceActivity)
                    : this.communityServiceActivitySvc.create('/communityServiceActivities', new CommunityServiceActivity(this.activityForm.value));

                let replyMsg = `Community service activity ${this.editMode ? 'updated' : 'created'} successfully!`;
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

        this.communityServiceActivitySvc.get('/communityServiceActivities').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.communityServiceActivities = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.communityServiceActivities.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the community service activities. Contact system administrator.'
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
