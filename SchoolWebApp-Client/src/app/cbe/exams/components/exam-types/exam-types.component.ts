import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {ExamType} from '../../models/exam-type';
import {ExamTypeService} from '../../services/exam-type.service';

@Component({
    selector: 'app-exam-types',
    templateUrl: './exam-types.component.html',
    styleUrl: './exam-types.component.scss'
})
export class ExamTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    page = 1;
    pageSize = 10;

    examTypeForm: FormGroup;
    buttonTitle: string = 'Add exam type';
    tableModel: string = 'examType';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/exam-types'], title: 'CBE Exams: Exam Types'}
    ];
    dashboardTitle = 'CBE Exams: Exam Types';
    tableTitle: string = 'Exam types list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Abbreviation', 'Internal', 'Rank', 'Description', 'Action'];

    editMode = false;
    examType: ExamType;
    collectionSize = 0;
    examTypes: ExamType[] = [];

    constructor(
        private formBuilder: FormBuilder,
        private toastr: ToastrService,
        private examTypeSvc: ExamTypeService
    ) {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.refreshItems();
        this.examTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: ['', [Validators.required]],
            internal: [false],
            rank: [0, [Validators.required]],
            description: ['']
        });
    }

    get f() {
        return this.examTypeForm.controls;
    }

    refreshItems = () => {
        this.examTypeSvc.get('/examTypes').subscribe({
            next: (examTypes) => {
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.collectionSize = this.examTypes.length;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.examTypeSvc.getById(id, '/examTypes').subscribe(
            (res) => {
                this.examType = new ExamType(res);
                this.examType.id = res.id;
                this.examTypeForm.setValue({
                    name: this.examType.name,
                    abbreviation: this.examType.abbreviation ?? '',
                    internal: this.examType.internal ?? false,
                    rank: this.examType.rank,
                    description: this.examType.description ?? ''
                });
                this.editMode = true;
                this.tableButton.onClick();
            },
            (err) => this.toastr.error(err.error)
        );
    }

    deleteItem(examType: ExamType) {
        Swal.fire({
            title: `Delete ${examType.name}?`,
            text: 'This action cannot be undone.',
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.examTypeSvc
                    .delete('/examTypes', parseInt(examType.id))
                    .subscribe(
                        () => {
                            this.refreshItems();
                            this.toastr.success('Exam type deleted successfully!');
                        },
                        (err) => this.toastr.error(err.error)
                    );
            }
        });
    }

    resetForm() {
        this.editMode = false;
        this.examTypeForm.reset();
        this.examTypeForm.patchValue({internal: false, rank: 0});
    }

    onSubmit = () => {
        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} exam type?`,
            text: `Confirm to ${this.editMode ? 'update' : 'add'} exam type.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new ExamType(this.examTypeForm.value);
                if (this.editMode) app.id = this.examType.id;
                let reqToProcess = this.editMode
                    ? this.examTypeSvc.update('/examTypes', app)
                    : this.examTypeSvc.create('/examTypes', app);
                reqToProcess.subscribe(
                    () => {
                        this.editMode = false;
                        this.closeButton.nativeElement.click();
                        this.refreshItems();
                        this.toastr.success('Exam type saved successfully!');
                        this.examTypeForm.reset();
                        this.examTypeForm.patchValue({internal: false, rank: 0});
                    },
                    (err) => this.toastr.error(err.error?.message || err.error)
                );
            }
        });
    };
}
