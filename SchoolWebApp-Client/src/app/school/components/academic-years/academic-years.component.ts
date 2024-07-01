import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {formatDate} from '@angular/common';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-academic-years',
    templateUrl: './academic-years.component.html',
    styleUrl: './academic-years.component.scss'
})
export class AcademicYearsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/academicYears'], title: 'Settings: Academic years'}
    ];

    dashboardTitle = 'Settings: Academic years';
    tableTitle: string = 'Academic years list';
    tableHeaders: string[] = [
        'Name',
        'Abbreviation',
        'Start Date',
        'End Date',
        'Description',
        'Status',
        'Action'
    ];
    formTitle: string = 'Academic years';
    academicYear: AcademicYear;
    academicYears: AcademicYear[] = [];
    tblShowViewButton: true;
    editMode = false;
    isAuthLoading: boolean;
    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;
    tableModel: string = 'academicYear';

    academicYearForm: FormGroup;

    constructor(
        private academicYearsSvc: AcademicYearsService,
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

    get f() {
        return this.academicYearForm.controls;
    }

    refreshItems() {
        this.academicYearForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: ['', [Validators.required]],
            startDate: [
                formatDate(
                    new Date(new Date().getFullYear(), 0, 1),
                    'yyyy-MM-dd',
                    'en'
                )
            ],
            endDate: [
                formatDate(
                    new Date(new Date().getFullYear(), 11, 31),
                    'yyyy-MM-dd',
                    'en'
                )
            ],
            description: [''],
            status: [false]
        });

        let academicYearsRequest = this.academicYearsSvc.get('/academicYears');

        forkJoin([academicYearsRequest]).subscribe(
            (res) => {
                this.collectionSize = res[0].length;
                this.academicYears= res[0].sort((a, b) => b.name.localeCompare(a.name));
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.academicYearsSvc.getById(id, '/academicYears').subscribe(
            (res) => {
                this.academicYear = new AcademicYear(res);
                this.academicYearForm.setValue({
                    name: this.academicYear.name,
                    abbreviation: this.academicYear.abbreviation,
                    startDate: formatDate(
                        new Date(this.academicYear.startDate),
                        'yyyy-MM-dd',
                        'en'
                    ),
                    endDate: formatDate(
                        new Date(this.academicYear.endDate),
                        'yyyy-MM-dd',
                        'en'
                    ),
                    description: this.academicYear.description,
                    status: this.academicYear.status
                });
                this.editMode = true;
                this.tableButton.onClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(id: number) {
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
                this.academicYearsSvc.delete('/academicYears', id).subscribe(
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

    onSubmit() {
        if (this.academicYearForm.invalid) {
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${
                this.editMode ? 'edit' : 'add'
            } record.`,
            width: 400,
            heightAuto: true,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let toSubmitDetails = new AcademicYear(
                    this.academicYearForm.value
                );
                if (this.editMode) toSubmitDetails.id = this.academicYear.id;

                let reqToProcess = this.editMode
                    ? this.academicYearsSvc.update(
                          '/academicYears',
                          toSubmitDetails
                      )
                    : this.academicYearsSvc.create(
                          '/academicYears',
                          toSubmitDetails
                      );

                let replyMsg = `Academic year ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.academicYearForm.reset();
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

    resetForm() {
        this.academicYearForm.reset();
    }
}
