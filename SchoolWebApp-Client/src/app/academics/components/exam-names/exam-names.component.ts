import {ExamName} from '@/academics/models/exam-name';
import {ExamType} from '@/academics/models/exam-type';
import {ExamNamesService} from '@/academics/services/exam-names.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-exam-names',
    templateUrl: './exam-names.component.html',
    styleUrl: './exam-names.component.scss'
})
export class ExamNamesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/examNames'], title: 'Settings: Exam names'}
    ];

    dashboardTitle = 'Settings:  Exam names';
    tableTitle: string = ' Exam names list';
    tableHeaders: string[] = [
        'Ref#',
        'Name',
        'Exam type',
        'Rank',
        'Description',
        'Action'
    ];

    examName: ExamName;
    examTypes: ExamType[] = [];
    examNames: ExamName[] = [];
    tblShowViewButton: true;
    editMode = false;
    isAuthLoading: boolean;
    page = 1;
    pageSize = 10;
    tableModel: string = 'examName';

    examNameForm: FormGroup;

    constructor(
        private examNamesSvc: ExamNamesService,
        private examTypesSvc: ExamTypesService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    get f() {
        return this.examNameForm.controls;
    }

    refreshItems() {
        this.examNameForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            examTypeId: [null, [Validators.required]],
            rank: [0],
            description: ['']
        });

        let examTypesRequest = this.examTypesSvc.get('/examTypes');
        let examNamesRequest = this.examNamesSvc.get('/examNames');

        forkJoin([examTypesRequest, examNamesRequest]).subscribe(
            ([examTypes, examNames]) => {
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.examNames = examNames.slice(
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
        this.examNamesSvc.getById(id, '/examNames').subscribe(
            (res) => {
                this.examName = new ExamName(res);
                this.examNameForm.setValue({
                    name: this.examName.name,
                    examTypeId: this.examName.examTypeId,
                    rank: this.examName.rank,
                    description: this.examName.description
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
                this.examNamesSvc.delete('/examNames', id).subscribe(
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
        if (this.examNameForm.invalid) {
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
                let itemToBeSaved = new ExamName(this.examNameForm.value);
                if (this.editMode) {
                    itemToBeSaved.id = this.examName.id;
                }

                let reqToProcess = this.editMode
                    ? this.examNamesSvc.update('/examNames', itemToBeSaved)
                    : this.examNamesSvc.create('/examNames', itemToBeSaved);

                let replyMsg = `Exam name ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.examNameForm.reset();
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
        this.examNameForm.reset();
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };
}
