import {ExamType} from '@/academics/models/exam-type';
import { ExamTypesService } from '@/academics/services/exam-types.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-exam-types',
    templateUrl: './exam-types.component.html',
    styleUrl: './exam-types.component.scss'
})
export class ExamTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/examTypes'], title: 'Settings: Exam type types'}
    ];

    dashboardTitle = 'Settings:  Exam type types';
    tableTitle: string = ' Exam types list';
    tableHeaders: string[] = [
        'Name',
        'Abbreviation',
        'Featured',
        'Description',
        'Action'
    ];

    examType: ExamType;
    examTypes: ExamType[] = [];
    tblShowViewButton: true;
    editMode = false;
    isAuthLoading: boolean;
    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;
    tableModel: string = 'examType';

    examTypeForm: FormGroup;

    constructor(
        private examTypesSvc: ExamTypesService,
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
        return this.examTypeForm.controls;
    }

    refreshItems() {
        this.examTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: [''],
            featured: [false],
            description: ['']
        });

        let examTypesRequest = this.examTypesSvc.get('/examTypes');

        forkJoin([examTypesRequest]).subscribe(
            (res) => {
                this.collectionSize = res[0].length;
                this.examTypes = res[0].slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.examTypesSvc.getById(id, '/examTypes').subscribe(
            (res) => {
                this.examType = new ExamType(res);
                this.examTypeForm.setValue({
                    name: this.examType.name,
                    abbreviation: this.examType.abbreviation,
                    featured: this.examType.featured,
                    description: this.examType.description
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
                this.examTypesSvc.delete('/examTypes', id).subscribe(
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
        if (this.examTypeForm.invalid) {
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
                let itemToBeSaved = new ExamType(this.examTypeForm.value);
                if (this.editMode) {
                    itemToBeSaved.id = this.examType.id;
                }

                let reqToProcess = this.editMode
                    ? this.examTypesSvc.update('/examTypes', itemToBeSaved)
                    : this.examTypesSvc.create('/examTypes', itemToBeSaved);

                let replyMsg = `Exam type type ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.examTypeForm.reset();
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
        this.examTypeForm.reset();
    }
}
