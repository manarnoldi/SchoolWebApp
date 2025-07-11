import {BreadCrumb} from '@/core/models/bread-crumb';
import {Religion} from '@/settings/models/religion';
import {ReligionsService} from '@/settings/services/religions.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-religions',
    templateUrl: './religions.component.html',
    styleUrl: './religions.component.scss'
})
export class ReligionsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    religionForm: FormGroup;

    buttonTitle: string = 'Add religion';
    tableModel: string = 'religion';
    tableTitle: string = 'Religions list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    religion: Religion;
    isAuthLoading: boolean;
    religions: Religion[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private religionsSvc: ReligionsService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'Religions list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/religions'], title: 'Settings:Religions'}
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
                this.religionsSvc.delete('/religions', id).subscribe(
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
        this.religionsSvc.getById(id, '/religions').subscribe(
            (res) => {
                this.religion = new Religion(res);
                this.religionForm.setValue({
                    name: this.religion.name,
                    rank: this.religion.rank,
                    description: this.religion.description
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
        if (this.religionForm.invalid) {
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
                    this.religion.name = this.religionForm.get('name').value;
                    this.religion.rank = this.religionForm.get('rank').value;
                    this.religion.description =
                        this.religionForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.religionsSvc.update('/religions', this.religion)
                    : this.religionsSvc.create(
                          '/religions',
                          new Religion(this.religionForm.value)
                      );

                let replyMsg = `Religion ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.religionForm.reset();
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
        return this.religionForm.controls;
    }

    refreshItems() {
        this.religionForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.religionsSvc.get('/religions').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.religions = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the religions. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    resetForm() {
        this.religionForm.reset();
        this.editMode = false;
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
