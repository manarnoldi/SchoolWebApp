import {BreadCrumb} from '@/core/models/bread-crumb';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
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

    staffCategoryForm: FormGroup;

    buttonTitle: string = 'Add staff category';
    tableModel: string = 'staffCategory';
    tableTitle: string = 'Staff categories list';
    tableHeaders: string[] = [
        'Ref#',
        'Name',
        'Code',
        'For teaching?',
        'Rank',
        'Description',
        'Action'
    ];

    editMode = false;
    staffCategory: StaffCategory;
    isAuthLoading: boolean;
    staffCategories: StaffCategory[] = [];
    tblShowViewButton: false;

    constructor(
        private staffCategoriesSvc: StaffCategoriesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
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
                    rank: this.staffCategory.rank,
                    forTeaching: this.staffCategory.forTeaching,
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
                    this.staffCategory.rank =
                        this.staffCategoryForm.get('rank').value;
                    this.staffCategory.forTeaching =
                        this.staffCategoryForm.get('forTeaching').value;
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
            rank: [0, [Validators.required]],
            forTeaching: [false],
            description: ['']
        });

        this.staffCategoriesSvc.get('/staffCategories').subscribe(
            (res) => {
                this.staffCategories = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.staffCategories.sort((a, b) => a.rank - b.rank);
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
        this.refreshItems();
    }

    resetForm() {
        this.staffCategoryForm.reset();
        this.editMode = false;
    }
    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
