import {Component, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {CoCurriculumScore} from '../../models/co-curriculum-score';
import {CoCurriculumScoreService} from '../../services/co-curriculum-score.service';
import {CoCurriculumScoreTypeService} from '../../services/co-curriculum-score-type.service';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';

@Component({
    selector: 'app-scores-setup',
    templateUrl: './scores-setup.component.html',
    styleUrl: './scores-setup.component.scss'
})
export class ScoresSetupComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    page = 1;
    pageSize = 10;

    scoreForm: FormGroup;
    buttonTitle: string = 'Add score';
    tableModel: string = 'scoresSetup';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/cocurriculum/scores-setup'], title: 'CBE Co-curricular: Scores Setup'}
    ];
    dashboardTitle = 'CBE Co-curricular: Scores Setup';
    tableTitle: string = 'Scores list';
    tableHeaders: string[] = ['Ref#', 'Score Name', 'Abbreviation', 'Score Type', 'Rank', 'Description', 'Action'];

    editMode = false;
    score: CoCurriculumScore;
    scores: CoCurriculumScore[] = [];
    scoreTypes: any[] = [];
    filterScoreTypeId: any = null;

    constructor(
        private formBuilder: FormBuilder,
        private toastr: ToastrService,
        private scoreSvc: CoCurriculumScoreService,
        private scoreTypeSvc: CoCurriculumScoreTypeService
    ) {}

    pageSizeChanged = (pageSize: number) => { this.pageSize = pageSize; };
    pageChanged = (page: number) => { this.page = page; };

    ngOnInit(): void {
        this.scoreForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            abbreviation: [''],
            rank: [0, [Validators.required]],
            description: [''],
            coCurriculumScoreTypeId: [null, [Validators.required]]
        });
        this.scoreTypeSvc.get('/coCurriculumScoreTypes').subscribe({
            next: (types) => { this.scoreTypes = types.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    }

    get f() { return this.scoreForm.controls; }

    onScoreTypeChange = () => {
        this.scores = [];
        if (!this.filterScoreTypeId) return;
        this.scoreSvc.get(`/coCurriculumScores/byScoreTypeId/${this.filterScoreTypeId}`).subscribe({
            next: (scores) => { this.scores = scores.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.scoreSvc.getById(id, '/coCurriculumScores').subscribe(
            (res) => {
                this.score = new CoCurriculumScore(res);
                this.score.id = res.id;
                this.scoreForm.setValue({
                    name: this.score.name,
                    abbreviation: this.score.abbreviation ?? '',
                    rank: this.score.rank,
                    description: this.score.description ?? '',
                    coCurriculumScoreTypeId: this.score.coCurriculumScoreTypeId
                });
                this.editMode = true;
                this.tableButton.onClick();
            },
            (err) => this.toastr.error(err.error)
        );
    }

    deleteItem(score: CoCurriculumScore) {
        Swal.fire({
            title: `Delete ${score.name}?`, text: 'This action cannot be undone.',
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.scoreSvc.delete('/coCurriculumScores', parseInt(score.id)).subscribe(
                    () => { this.onScoreTypeChange(); this.toastr.success('Score deleted!'); },
                    (err) => this.toastr.error(err.error)
                );
            }
        });
    }

    resetForm() {
        this.editMode = false;
        this.scoreForm.reset();
        this.scoreForm.patchValue({rank: 0, coCurriculumScoreTypeId: this.filterScoreTypeId});
    }

    onSubmit = () => {
        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} score?`,
            text: `Confirm to ${this.editMode ? 'update' : 'add'} score.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: this.editMode ? 'Update' : 'Add', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new CoCurriculumScore(this.scoreForm.value);
                if (this.editMode) app.id = this.score.id;
                let req = this.editMode
                    ? this.scoreSvc.update('/coCurriculumScores', app)
                    : this.scoreSvc.create('/coCurriculumScores', app);
                req.subscribe(
                    () => {
                        this.editMode = false;
                        this.closeButton.nativeElement.click();
                        this.onScoreTypeChange();
                        this.toastr.success('Score saved!');
                        this.resetForm();
                    },
                    (err) => this.toastr.error(err.error?.message || err.error)
                );
            }
        });
    };
}
