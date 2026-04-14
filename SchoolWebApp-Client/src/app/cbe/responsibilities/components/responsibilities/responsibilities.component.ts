import {BreadCrumb} from '@/core/models/bread-crumb';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Responsibility} from '../../models/responsibility';
import {ResponsibilityService} from '../../services/responsibility.service';

@Component({
    selector: 'app-responsibilities',
    templateUrl: './responsibilities.component.html',
    styleUrl: './responsibilities.component.scss'
})
export class ResponsibilitiesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    responsibilityForm: FormGroup;

    buttonTitle: string = 'Add responsibility';
    tableModel: string = 'responsibility';
    tableTitle: string = 'Responsibilities list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    responsibility: Responsibility;
    isAuthLoading: boolean;
    responsibilities: Responsibility[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private responsibilitySvc: ResponsibilityService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Responsibilities: Responsibilities List';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/responsibilities/responsibilities'], title: 'CBE Responsibilities: Responsibilities List'}
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
                this.responsibilitySvc.delete('/responsibilities', id).subscribe(
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
        this.responsibilitySvc.getById(id, '/responsibilities').subscribe(
            (res) => {
                this.responsibility = new Responsibility(res);
                this.responsibilityForm.setValue({
                    name: this.responsibility.name,
                    rank: this.responsibility.rank,
                    description: this.responsibility.description
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
        if (this.responsibilityForm.invalid) {
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
                    this.responsibility.name = this.responsibilityForm.get('name').value;
                    this.responsibility.description = this.responsibilityForm.get('description').value;
                    this.responsibility.rank = this.responsibilityForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.responsibilitySvc.update('/responsibilities', this.responsibility)
                    : this.responsibilitySvc.create('/responsibilities', new Responsibility(this.responsibilityForm.value));

                let replyMsg = `Responsibility ${this.editMode ? 'updated' : 'created'} successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.responsibilityForm.reset();
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
        return this.responsibilityForm.controls;
    }

    refreshItems() {
        this.responsibilityForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.responsibilitySvc.get('/responsibilities').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.responsibilities = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.responsibilities.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the responsibilities. Contact system administrator.'
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
        this.responsibilityForm.reset();
        this.editMode = false;
    }
}
