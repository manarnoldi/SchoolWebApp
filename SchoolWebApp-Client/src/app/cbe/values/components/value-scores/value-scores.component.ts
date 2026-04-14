import {BreadCrumb} from '@/core/models/bread-crumb';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {ValueScore} from '../../models/value-score';
import {ValueScoreService} from '../../services/value-score.service';

@Component({
    selector: 'app-value-scores',
    templateUrl: './value-scores.component.html',
    styleUrl: './value-scores.component.scss'
})
export class ValueScoresComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    page = 1;
    pageSize = 10;

    valueScoreForm: FormGroup;

    buttonTitle: string = 'Add value score';
    tableModel: string = 'valueScore';
    tableTitle: string = 'Value scores list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Abbreviation', 'Rank', 'Description', 'Action'];

    editMode = false;
    valueScore: ValueScore;
    isAuthLoading: boolean;
    valueScores: ValueScore[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private valueScoreSvc: ValueScoreService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Values: Value Scores';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/values/values-scores'], title: 'CBE Values: Value Scores'}
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
                this.valueScoreSvc.delete('/valueScores', id).subscribe(
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
        this.valueScoreSvc.getById(id, '/valueScores').subscribe(
            (res) => {
                this.valueScore = new ValueScore(res);
                this.valueScoreForm.setValue({
                    name: this.valueScore.name,
                    abbreviation: this.valueScore.abbreviation ?? '',
                    rank: this.valueScore.rank,
                    description: this.valueScore.description
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
        if (this.valueScoreForm.invalid) {
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
                    this.valueScore.name = this.valueScoreForm.get('name').value;
                    this.valueScore.abbreviation = this.valueScoreForm.get('abbreviation').value;
                    this.valueScore.description = this.valueScoreForm.get('description').value;
                    this.valueScore.rank = this.valueScoreForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.valueScoreSvc.update('/valueScores', this.valueScore)
                    : this.valueScoreSvc.create('/valueScores', new ValueScore(this.valueScoreForm.value));

                let replyMsg = `Value score ${this.editMode ? 'updated' : 'created'} successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.valueScoreForm.reset();
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
        return this.valueScoreForm.controls;
    }

    refreshItems() {
        this.valueScoreForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: [''],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.valueScoreSvc.get('/valueScores').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.valueScores = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.valueScores.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the value scores. Contact system administrator.'
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
        this.valueScoreForm.reset();
        this.editMode = false;
    }
}
