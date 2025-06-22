import {ClassLeadershipRole} from '@/class/models/class-leadership-role';
import {ClassLeadershipRolesService} from '@/class/services/class-leadership-roles.service';
import {PersonType} from '@/core/enums/personTypes';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-class-leadership-roles',
    templateUrl: './class-leadership-roles.component.html',
    styleUrl: './class-leadership-roles.component.scss'
})
export class ClassLeadershipRolesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    page = 1;
    pageSize = 10;

    classLeadershipRoleForm: FormGroup;

    buttonTitle: string = 'Add class leadership role';
    tableModel: string = 'classLeadershipRole';
    tableTitle: string = 'Class leadership roles list';
    tableHeaders: string[] = [
        'Name',
        'Person type',
        'Rank',
        'Description',
        'Action'
    ];

    editMode = false;
    classLeadershipRole: ClassLeadershipRole;
    isAuthLoading: boolean;
    classLeadershipRoles: ClassLeadershipRole[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    personType = PersonType;
    personTypes;

    constructor(
        private classLeadershipRoleSvc: ClassLeadershipRolesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {
        this.personTypes = Object.keys(this.personType).filter((k) =>
            isNaN(Number(k))
        );
    }
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
                    .delete('/classLeadershipRoles', id)
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
        this.classLeadershipRoleSvc
            .getById(id, '/classLeadershipRoles')
            .subscribe(
                (res) => {
                    let classLeadershipRoleId = id;
                    this.classLeadershipRole = new ClassLeadershipRole(res);
                    this.classLeadershipRole.id = classLeadershipRoleId;
                    this.classLeadershipRoleForm.setValue({
                        name: this.classLeadershipRole.name,
                        rank: this.classLeadershipRole.rank ?? 0,
                        description: this.classLeadershipRole.description,
                        personType:
                            this.personTypes[
                                this.classLeadershipRole.personType
                            ]
                    });
                    this.editMode = true;
                    this.tableButton.onClick();
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
                let app = new ClassLeadershipRole(
                    this.classLeadershipRoleForm.value
                );
                // let personTypeNum = this.personTypes[app.personType];
                // app.personType = personTypeNum;
                if (this.editMode) app.id = this.classLeadershipRole.id;
                let reqToProcess = this.editMode
                    ? this.classLeadershipRoleSvc.update(
                          '/classLeadershipRoles',
                          app
                      )
                    : this.classLeadershipRoleSvc.create(
                          '/classLeadershipRoles',
                          app
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
            rank: [0, [Validators.required]],
            description: [''],
            personType: [this.personTypes[0], [Validators.required]]
        });

        this.classLeadershipRoleSvc.get('/classLeadershipRoles').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.classLeadershipRoles = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.classLeadershipRoles = this.classLeadershipRoles.sort(
                    (a, b) => a.rank - b.rank
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
        this.refreshItems();
    }

    resetForm() {
        this.classLeadershipRoleForm.reset();
        this.editMode = false;
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
