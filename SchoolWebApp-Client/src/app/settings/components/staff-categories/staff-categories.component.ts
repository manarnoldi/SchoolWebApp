import {BreadCrumb} from '@/core/models/bread-crumb';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import { TableButtonComponent } from '@/shared/directives/table-button/table-button.component';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-staff-categories',
    templateUrl: './staff-categories.component.html',
    styleUrl: './staff-categories.component.scss'
})
export class StaffCategoriesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    
    page = 1;
    pageSize = 10;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    staffCategoryForm: FormGroup;

    buttonTitle: string = 'Add staff category';
    tableModel: string = 'staffCategory';
    tableTitle: string = 'Staff categories list';
    tableHeaders: string[] = ['Name','Code', 'Description', 'Action'];

    editMode = false;
    staffCategory: StaffCategory;
    isAuthLoading: boolean;
    staffCategories: StaffCategory[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private staffCategoriesSvc: StaffCategoriesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder,
        private tableSettingsSvc: TableSettingsService
    ) {}
    closeResult = '';
    dashboardTitle = 'Staff category list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/settings/staffCategories'],
            title: 'Settings:Staff categories'
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
                this.staffCategoriesSvc
                    .delete('/staffCategories', id)
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
        this.staffCategoriesSvc.getById(id, '/staffCategories').subscribe(
            (res) => {
                this.staffCategory = new StaffCategory(res);
                this.staffCategoryForm.setValue({
                    code: this.staffCategory.code,
                    name: this.staffCategory.name,
                    description: this.staffCategory.description
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
        if (this.staffCategoryForm.invalid) {
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
                    this.staffCategory.name =
                        this.staffCategoryForm.get('name').value;
                    this.staffCategory.code =
                        this.staffCategoryForm.get('code').value;
                    this.staffCategory.description =
                        this.staffCategoryForm.get('description').value;
                }

                let reqToProcess = this.editMode
                    ? this.staffCategoriesSvc.update(
                          '/staffCategories',
                          this.staffCategory
                      )
                    : this.staffCategoriesSvc.create(
                          '/staffCategories',
                          new StaffCategory(this.staffCategoryForm.value)
                      );

                let replyMsg = `Staff category ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.staffCategoryForm.reset();
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
        return this.staffCategoryForm.controls;
    }

    refreshItems() {
        this.staffCategoryForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            code: ['', [Validators.required]],
            description: ['']
        });

        this.staffCategoriesSvc.get('/staffCategories').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.staffCategories = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the staff categories. Contact system administrator.'
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
        this.staffCategoryForm.reset();
        this.editMode = false;
    }
}
