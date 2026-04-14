import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ThemeAddFormComponent} from './theme-add-form/theme-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Theme} from '../../models/theme';
import {ThemeService} from '../../services/theme.service';
import {Subject} from '@/academics/models/subject';
import {SubjectsService} from '@/academics/services/subjects.service';
import {AcademicYear} from '@/school/models/academic-year';
import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {LearningLevel} from '@/class/models/learning-level';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';

@Component({
    selector: 'app-themes',
    templateUrl: './themes.component.html',
    styleUrl: './themes.component.scss'
})
export class ThemesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(ThemeAddFormComponent)
    themeFormComp: ThemeAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'theme';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/themes'], title: 'Themes'}
    ];
    dashboardTitle = 'CBE Assessments: Themes';
    tableTitle: string = ' Themes list';
    tableHeaders: string[] = [
        'Ref#',
        'Curriculum',
        'Learning Level',
        'Subject',
        'Code',
        'Theme Name',
        'Description',
        'Rank',
        'Action'
    ];

    theme: Theme;
    themes: Theme[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    selectedCurriculum: Curriculum;

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private themeSvc: ThemeService,
        private subjectsSvc: SubjectsService,
        private curriculaSvc: CurriculumService,
        private learninglevelSvc: LearningLevelsService,
        private educationlevelSvc: EducationLevelService,
        private educationlevelSubjectsSvc: EducationLevelSubjectService,
        private academicYearSvc: AcademicYearsService
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

    searchThemes = (
        acadId: number,
        currId: number,
        llId: number,
        subjId: number
    ) => {
        let cysPass = new SchoolSoftFilter();
        cysPass.academicYearId = acadId;
        cysPass.curriculumId = currId;
        cysPass.learningLevelId = llId;
        cysPass.subjectId = subjId;
        this.firstLoad = true;
        this.ssFFormComp.setFormControls(cysPass);
        this.ssFFormComp.onSubmit();
        this.isAuthLoading = false;
        this.themeFormComp.editMode = false;
    };

    refreshItems() {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearSvc.get('/academicYears');
        let educationLevelsReq = this.educationlevelSvc.get('/educationLevels');

        forkJoin([
            curriculaReq,
            academicYearsReq,
            educationLevelsReq
        ]).subscribe({
            next: ([curricula, academicYears, educationLevels]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);

                this.academicYears = academicYears.sort(
                    (a, b) => a.rank - b.rank
                );
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );

                let topAcademicYear = this.academicYears.find(
                    (y) => y.status == true
                );
                let topCurriculum = this.curricula[0];

                this.learninglevelSvc
                    .getLearningLevelsByCurriculum(parseInt(topCurriculum.id))
                    .subscribe({
                        next: (learnLvls) => {
                            this.learningLevels = learnLvls.sort(
                                (a, b) => a.rank - b.rank
                            );
                            let topLearningLevel = this.learningLevels[0];
                            this.educationlevelSubjectsSvc
                                .getByEducationLevelAndAcademicYear(
                                    topLearningLevel.educationLevelId,
                                    parseInt(topAcademicYear.id)
                                )
                                .subscribe({
                                    next: (elSubjects) => {
                                        this.subjects = elSubjects
                                            .map((es) => es.subject)
                                            .sort((a, b) => a.rank - b.rank);
                                        let topSubject = this.subjects[0];
                                        this.searchThemes(
                                            parseInt(topAcademicYear.id),
                                            parseInt(topCurriculum.id),
                                            parseInt(topLearningLevel.id),
                                            parseInt(topSubject.id)
                                        );
                                    },
                                    error: (err) => this.toastr.error(err.error)
                                });
                        },
                        error: (err) => this.toastr.error(err.error)
                    });
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }

    academicYearAddFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = [];
        if (!acadYearId) {
            return;
        }
        let curId = this.themeFormComp.themeForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.themes = [];
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        if (!acadYearId) {
            return;
        }
        let curId =
            this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = [];
        let acadYearId =
            this.themeFormComp.themeForm.get('academicYearId')?.value;

        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumFilterFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = this.themes = [];
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        let acadYearId =
            this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;

        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumAcademicYearChanged = (currId: number, acadId: number) => {
        let learnLvlsReq =
            this.learninglevelSvc.getLearningLevelsByCurriculum(currId);

        forkJoin([learnLvlsReq]).subscribe({
            next: ([learnLvls]) => {
                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelAddFormChanged = (learnLvlId: number) => {
        this.subjects = [];
        let acadYearId =
            this.themeFormComp.themeForm.get('academicYearId')?.value;
        if (!learnLvlId || !acadYearId) return;
        let learnLvl = this.learningLevels.find(
            (l) => parseInt(l.id) == learnLvlId
        );
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(
                learnLvl.educationLevelId,
                acadYearId
            )
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects
                        .map((es) => es.subject)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    learningLevelFilterFormChanged = (learnLvlId: number) => {
        this.subjects = this.themes = [];
        let learnLvl = this.learningLevels.find(
            (l) => parseInt(l.id) == learnLvlId
        );
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId').setValue(null);
        let acadYearId =
            this.ssFFormComp.schoolSoftFilterForm.get('academicYearId').value;
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(
                learnLvl.educationLevelId,
                acadYearId
            )
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects
                        .map((es) => es.subject)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    subjectFilterFormChanged = (id: number) => {
        this.themes = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/themes/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.themeSvc.get(searchStr).subscribe({
            next: (themes) => {
                this.themes = themes.sort((a, b) => a.rank - b.rank);
                if (this.themes.length <= 0 && !this.firstLoad) {
                    this.toastr.info(
                        'No record found for the selected search items!'
                    );
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.themeSvc.getById(id, '/themes').subscribe(
            (res) => {
                let themeId = res.id;
                this.theme = new Theme(res);
                this.theme.id = themeId;

                let learningLevelreq = this.learninglevelSvc.getById(
                    this.theme.learningLevelId,
                    '/learningLevels'
                );
                let subjectreq = this.subjectsSvc.getById(
                    this.theme.subjectId,
                    '/subjects'
                );

                forkJoin([learningLevelreq, subjectreq]).subscribe({
                    next: ([learningLevel, subject]) => {
                        this.learningLevels = [];
                        this.learningLevels.push(learningLevel);
                        this.theme.learningLevel = learningLevel;
                        this.subjects = [];
                        this.subjects.push(subject);
                        this.theme.subject = subject;
                        if (this.theme.curriculum) {
                            this.curricula = [];
                            this.curricula.push(this.theme.curriculum);
                        }
                        this.themeFormComp.setFormControls(this.theme);
                        this.themeFormComp.editMode = true;
                        this.themeFormComp.theme = this.theme;
                        this.tableButton.onClick();
                    },
                    error: (err) => this.toastr.error(err.error)
                });
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(theme: Theme) {
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
                this.themeSvc
                    .delete('/themes', parseInt(theme.id))
                    .subscribe(
                        (res) => {
                            let acadYearId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'academicYearId'
                                ).value;
                            let currId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'curriculumId'
                                ).value;
                            let llId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'learningLevelId'
                                ).value;
                            let subjId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'subjectId'
                                ).value;

                            this.searchThemes(
                                acadYearId,
                                currId,
                                llId,
                                subjId
                            );
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
        this.themeFormComp.editMode = false;
        this.themeFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addTheme = (theme: Theme) => {
        Swal.fire({
            title: `${this.themeFormComp.editMode ? 'Update' : 'Add'} theme?`,
            text: `Confirm if you want to ${
                this.themeFormComp.editMode ? 'update' : 'add'
            } theme.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.themeFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new Theme(theme);
                if (this.themeFormComp.editMode) app.id = theme.id;
                let reqToProcess = this.themeFormComp.editMode
                    ? this.themeSvc.update('/themes', app)
                    : this.themeSvc.create('/themes', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.themeFormComp.editMode = false;
                        this.themeFormComp.refreshItems();
                        this.toastr.success('Theme saved successfully');
                        this.searchThemes(
                            null,
                            theme.curriculumId,
                            theme.learningLevelId,
                            theme.subjectId
                        );
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
