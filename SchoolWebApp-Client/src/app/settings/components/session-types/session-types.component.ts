import {BreadCrumb} from '@/core/models/bread-crumb';
import {SessionType} from '@/settings/models/session-type';
import {SessionTypesService} from '@/settings/services/session-types.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-session-types',
    templateUrl: './session-types.component.html',
    styleUrl: './session-types.component.scss'
})
export class SessionTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    sessionTypeForm: FormGroup;

    buttonTitle: string = 'Add session type';
    tableModel: string = 'sessionType';
    tableTitle: string = 'Session types list';
    tableHeaders: string[] = ['Name', 'Description', 'Action'];

    editMode = false;
    sessionType: SessionType;
    isAuthLoading: boolean;
    sessionTypes: SessionType[] = [];
    tblShowViewButton: false;

    constructor(
        private sessionTypesSvc: SessionTypesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Session types list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/sessionTypes'], title: 'Settings:Session types'}
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
                this.sessionTypesSvc.delete('/sessionTypes', id).subscribe(
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
        this.sessionTypesSvc.getById(id, '/sessionTypes').subscribe(
            (res) => {
                this.sessionType = new SessionType(res);
                this.sessionTypeForm.setValue({
                    name: this.sessionType.name,
                    description: this.sessionType.description
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
        if (this.sessionTypeForm.invalid) {
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
                    this.sessionType.name =
                        this.sessionTypeForm.get('name').value;
                    this.sessionType.description =
                        this.sessionTypeForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.sessionTypesSvc.update(
                          '/sessionTypes',
                          this.sessionType
                      )
                    : this.sessionTypesSvc.create(
                          '/sessionTypes',
                          new SessionType(this.sessionTypeForm.value)
                      );

                let replyMsg = `SessionType ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.sessionTypeForm.reset();
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
        return this.sessionTypeForm.controls;
    }

    refreshItems() {
        this.sessionTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: ['']
        });

        this.sessionTypesSvc.get('/sessionTypes').subscribe(
            (res) => {
                this.sessionTypes = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the session types. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    resetForm() {
        this.sessionTypeForm.reset();
        this.editMode = false;
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
