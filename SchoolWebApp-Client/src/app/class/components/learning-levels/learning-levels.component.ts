import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {LearningLevelsFormComponent} from './learning-levels-form/learning-levels-form.component';
import {forkJoin} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {EducationLevel} from '@/school/models/educationLevel';
import {ToastrService} from 'ngx-toastr';
import {EducationLevelService} from '@/school/services/education-level.service';
import Swal from 'sweetalert2';
import {LearningLevel} from '@/class/models/learning-level';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {Curriculum} from '@/academics/models/curriculum';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-learning-levels',
    templateUrl: './learning-levels.component.html',
    styleUrl: './learning-levels.component.scss'
})
export class LearningLevelsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(LearningLevelsFormComponent)
    learningLevelForm: LearningLevelsFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'learningLevel';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/class/learningLevels'], title: 'Class: Learning Levels'}
    ];
    dashboardTitle = 'Class: Learning Levels';
    tableTitle: string = ' Learning levels list';
    tableHeaders: string[] = [
        'Name',
        'Education level',
        'Rank',
        'Description',
        'Action'
    ];

    learningLevel: LearningLevel;
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];
    curricula: Curriculum[] = [];

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSvc: EducationLevelService,
        private curriculumSvc: CurriculumService
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
        let curriculaReq = this.curriculumSvc.get('/curricula');
        let educationLevelReq = this.educationLevelSvc.get('/educationLevels');

        forkJoin([curriculaReq, educationLevelReq]).subscribe(
            ([curricula, educationLevels]) => {
                this.educationLevels = educationLevels;
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                const topCurriculum = this.curricula[0];

                let cysPass = new SchoolSoftFilter();
                cysPass.curriculumId = parseInt(topCurriculum.id);

                this.ssFilterFormComponent.setFormControls(cysPass);
                this.ssFilterFormComponent.onSubmit();

                this.isAuthLoading = false;
                this.learningLevelForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    curriculumChanged = (id: number) => {
        this.learningLevels = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/learningLevels/byCurriculumId?curriculumId=${cys.curriculumId ?? ''}`;
        this.learningLevelSvc.get(searchStr).subscribe({
            next: (learningLevels) => {
                this.learningLevels = learningLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                if (this.learningLevels.length <= 0 && !this.firstLoad) {
                    this.toastr.info(
                        'No record found for the selected curriculum!'
                    );
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.learningLevelSvc.getById(id, '/learningLevels').subscribe(
            (res) => {
                let learningLevelId = res.id;
                this.learningLevel = new LearningLevel(res);
                this.learningLevel.id = learningLevelId;
                this.learningLevelForm.setFormControls(this.learningLevel);
                this.learningLevelForm.editMode = true;
                this.learningLevelForm.learningLevel = this.learningLevel;
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
                this.learningLevelSvc.delete('/learningLevels', id).subscribe(
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
        this.learningLevelForm.editMode = false;
        this.learningLevelForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addLearningLevel = (learningLevel: LearningLevel) => {
        Swal.fire({
            title: `${this.learningLevelForm.editMode ? 'Update' : 'Add'} learning Level?`,
            text: `Confirm if you want to ${
                this.learningLevelForm.editMode ? 'update' : 'add'
            } learning Level.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.learningLevelForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new LearningLevel(learningLevel);
                if (this.learningLevelForm.editMode) app.id = learningLevel.id;
                let reqToProcess = this.learningLevelForm.editMode
                    ? this.learningLevelSvc.update('/learningLevels', app)
                    : this.learningLevelSvc.create('/learningLevels', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.learningLevelForm.editMode = false;
                        this.learningLevelForm.refreshItems();
                        this.toastr.success(
                            'Learning level saved successfully'
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
