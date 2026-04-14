import {BreadCrumb} from '@/core/models/bread-crumb';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Value} from '../../models/value';
import {ValueService} from '../../services/value.service';

@Component({
    selector: 'app-values',
    templateUrl: './values.component.html',
    styleUrl: './values.component.scss'
})
export class ValuesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    valueForm: FormGroup;

    buttonTitle: string = 'Add value';
    tableModel: string = 'value';
    tableTitle: string = 'Values list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    value: Value;
    isAuthLoading: boolean;
    values: Value[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private valueSvc: ValueService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Values: Values Register';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/values/values-register'], title: 'CBE Values: Values Register'}
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
                this.valueSvc.delete('/values', id).subscribe(
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
        this.valueSvc.getById(id, '/values').subscribe(
            (res) => {
                this.value = new Value(res);
                this.valueForm.setValue({
                    name: this.value.name,
                    rank: this.value.rank,
                    description: this.value.description
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
        if (this.valueForm.invalid) {
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
                    this.value.name = this.valueForm.get('name').value;
                    this.value.description = this.valueForm.get('description').value;
                    this.value.rank = this.valueForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.valueSvc.update('/values', this.value)
                    : this.valueSvc.create('/values', new Value(this.valueForm.value));

                let replyMsg = `Value ${this.editMode ? 'updated' : 'created'} successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.valueForm.reset();
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
        return this.valueForm.controls;
    }

    refreshItems() {
        this.valueForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.valueSvc.get('/values').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.values = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.values.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the values. Contact system administrator.'
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
        this.valueForm.reset();
        this.editMode = false;
    }
}
