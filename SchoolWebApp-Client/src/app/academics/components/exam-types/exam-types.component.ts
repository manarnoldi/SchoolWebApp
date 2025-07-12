import {ExamType} from '@/academics/models/exam-type';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
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
        {link: ['/settings/examTypes'], title: 'Settings: Exam types'}
    ];

    dashboardTitle = 'Settings:  Exam types';
    tableTitle: string = ' Exam types list';
    tableHeaders: string[] = [
        'Ref#',
        'Name',
        'Abbreviation',
        'Rank',
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
    tableModel: string = 'examType';

    examTypeForm: FormGroup;

    constructor(
        private examTypesSvc: ExamTypesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    get f() {
        return this.examTypeForm.controls;
    }

    refreshItems() {
        this.examTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: [''],
            rank: [0],
            featured: [false],
            description: ['']
        });

        let examTypesRequest = this.examTypesSvc.get('/examTypes');

        forkJoin([examTypesRequest]).subscribe(
            (res) => {
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
                    rank: this.examType.rank,
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

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
