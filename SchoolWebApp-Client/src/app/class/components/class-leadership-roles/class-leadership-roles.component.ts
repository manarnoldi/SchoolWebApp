import { ClassLeadershipRole } from '@/class/models/class-leadership-role';
import { ClassLeadershipRolesService } from '@/class/services/class-leadership-roles.service';
import { BreadCrumb } from '@/core/models/bread-crumb';
import { SettingsTableComponent } from '@/shared/directives/settings-table/settings-table.component';
import { TableSettingsService } from '@/shared/services/table-settings.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { forkJoin, Subscription } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-class-leadership-roles',
    templateUrl: './class-leadership-roles.component.html',
    styleUrl: './class-leadership-roles.component.scss'
})
export class ClassLeadershipRolesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    classLeadershipRoleForm: FormGroup;

    buttonTitle: string = 'Add class leadership role';
    tableModel: string = 'classLeadershipRole';
    tableTitle: string = 'Class leadership roles list';
    tableHeaders: string[] = ['Name', 'Description', 'Action'];

    editMode = false;
    classLeadershipRole: ClassLeadershipRole;
    isAuthLoading: boolean;
    classLeadershipRoles: ClassLeadershipRole[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private classLeadershipRoleSvc: ClassLeadershipRolesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private tableSettingsSvc: TableSettingsService
    ) {}
    closeResult = '';
    dashboardTitle = 'Class leadership roles list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/class/leadership-roles'], title: 'Class: Leadership Roles'}
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
                this.classLeadershipRoleSvc
                    .delete('/classLeadershipRoleRoles', id)
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
        this.classLeadershipRoleSvc.getById(id, '/classLeadershipRoles').subscribe(
            (res) => {
                this.classLeadershipRole = new ClassLeadershipRole(res);
                this.classLeadershipRoleForm.setValue({
                    name: this.classLeadershipRole.name,
                    description: this.classLeadershipRole.description
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
        if (this.classLeadershipRoleForm.invalid) {
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
                    this.classLeadershipRole.name =
                        this.classLeadershipRoleForm.get('name').value;
                    this.classLeadershipRole.description =
                        this.classLeadershipRoleForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.classLeadershipRoleSvc.update(
                          '/classLeadershipRoles',
                          this.classLeadershipRole
                      )
                    : this.classLeadershipRoleSvc.create(
                          '/classLeadershipRoles',
                          new ClassLeadershipRole(this.classLeadershipRoleForm.value)
                      );

                let replyMsg = `Class leadership roles ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.classLeadershipRoleForm.reset();
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
        return this.classLeadershipRoleForm.controls;
    }

    refreshItems() {
        this.classLeadershipRoleForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: ['']
        });

        this.classLeadershipRoleSvc.get('/classLeadershipRoles').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.classLeadershipRoles = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the class leadership roles. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
        this.refreshItems();
    }

    resetForm() {
        this.classLeadershipRoleForm.reset();
        this.editMode = false;
    }
}
