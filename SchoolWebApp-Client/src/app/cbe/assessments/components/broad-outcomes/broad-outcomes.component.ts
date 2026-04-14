import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {BroadOutcomeAddFormComponent} from './broad-outcome-add-form/broad-outcome-add-form.component';
import {BroadOutcome} from '../../models/broad-outcome';
import {ToastrService} from 'ngx-toastr';
import {BroadOutcomeService} from '../../services/broad-outcome.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {EducationLevel} from '@/school/models/educationLevel';
import {Subject} from '@/academics/models/subject';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';

@Component({
    selector: 'app-broad-outcomes',
    templateUrl: './broad-outcomes.component.html',
    styleUrl: './broad-outcomes.component.scss'
})
export class BroadOutcomesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(BroadOutcomeAddFormComponent)
    broadOutcomeForm: BroadOutcomeAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'broadOutcome';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/broad-outcomes'], title: 'Broad/Subject Outcomes'}
    ];
    dashboardTitle = 'CBE Assessments: Broad Outcomes';
    tableTitle: string = ' Broad Outcomes list';
    tableHeaders: string[] = [
        'Ref#',
        'Education Level',
        'Subject',
        'Outcome Name',
        'Description',
        'Rank',
        'Action'
    ];

    broadOutcome: BroadOutcome;
    broadOutcomes: BroadOutcome[] = [];
    educationLevels: EducationLevel[] = [];
    subjects: Subject[] = [];

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private broadOutcomeSvc: BroadOutcomeService,
        private educationLevelSvc: EducationLevelService,
        private subjectsSvc: SubjectsService
    ) {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let educationLevelReq = this.educationLevelSvc.get('/educationLevels');
        let subjectsReq = this.subjectsSvc.get('/subjects');

        forkJoin([educationLevelReq, subjectsReq]).subscribe(
            ([educationLevels, subjects]) => {
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                this.subjects = subjects.sort((a, b) => a.rank - b.rank);

                const topEducationLevel = this.educationLevels[0];
                const topSubject = this.subjects[0];

                let cysPass = new SchoolSoftFilter();
                cysPass.educationLevelId = parseInt(topEducationLevel.id);
                cysPass.subjectId = parseInt(topSubject.id);

                this.ssFilterFormComponent.setFormControls(cysPass);
                this.ssFilterFormComponent.onSubmit();

                this.isAuthLoading = false;
                this.broadOutcomeForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    educationLevelChanged = (id: number) => {
        this.broadOutcomes = [];
    };

    subjectChanged = (id: number) => {
        this.broadOutcomes = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/broadOutcomes/byEducationLevelIdSubjectId/${cys.educationLevelId ?? ''}/${cys.subjectId ?? ''}`;
        this.broadOutcomeSvc.get(searchStr).subscribe({
            next: (broadOutcomes) => {
                this.broadOutcomes = broadOutcomes.sort(
                    (a, b) => a.rank - b.rank
                );
                if (this.broadOutcomes.length <= 0 && !this.firstLoad) {
                    this.toastr.info(
                        'No record found for the selected education level and subject!'
                    );
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.broadOutcomeSvc.getById(id, '/broadOutcomes').subscribe(
            (res) => {
                let broadOutcomeId = res.id;
                this.broadOutcome = new BroadOutcome(res);
                this.broadOutcome.id = broadOutcomeId;
                this.broadOutcomeForm.setFormControls(this.broadOutcome);
                this.broadOutcomeForm.editMode = true;
                this.broadOutcomeForm.broadOutcome = this.broadOutcome;
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
                this.broadOutcomeSvc.delete('/broadOutcomes', id).subscribe(
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

    resetForm = () => {
        this.broadOutcomeForm.editMode = false;
        this.broadOutcomeForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addBroadOutcome = (broadOutcome: BroadOutcome) => {
        Swal.fire({
            title: `${this.broadOutcomeForm.editMode ? 'Update' : 'Add'} broad outcome?`,
            text: `Confirm if you want to ${
                this.broadOutcomeForm.editMode ? 'update' : 'add'
            } broad outcome.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.broadOutcomeForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new BroadOutcome(broadOutcome);
                if (this.broadOutcomeForm.editMode)
                    app.id = broadOutcome.id;
                let reqToProcess = this.broadOutcomeForm.editMode
                    ? this.broadOutcomeSvc.update('/broadOutcomes', app)
                    : this.broadOutcomeSvc.create('/broadOutcomes', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.broadOutcomeForm.editMode = false;
                        this.broadOutcomeForm.refreshItems();
                        this.toastr.success(
                            'Broad outcome saved successfully'
                        );
                        this.refreshItems();
                        this.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
