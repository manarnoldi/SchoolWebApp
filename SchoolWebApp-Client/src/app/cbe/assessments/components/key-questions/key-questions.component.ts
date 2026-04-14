import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {KeyQuestionAddFormComponent} from './key-question-add-form/key-question-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {KeyQuestion} from '../../models/key-question';
import {KeyQuestionService} from '../../services/key-question.service';
import {SubStrand} from '../../models/sub-strand';
import {SubStrandService} from '../../services/sub-strand.service';
import {Strand} from '../../models/strand';
import {StrandService} from '../../services/strand.service';
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
    selector: 'app-key-questions',
    templateUrl: './key-questions.component.html',
    styleUrl: './key-questions.component.scss'
})
export class KeyQuestionsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(KeyQuestionAddFormComponent)
    keyQuestionFormComp: KeyQuestionAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'keyQuestion';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/key-questions'], title: 'Key Questions'}
    ];
    dashboardTitle = 'CBE Assessments: Key Questions';
    tableTitle: string = ' Key Questions list';
    tableHeaders: string[] = [
        'Ref#',
        'Key Question',
        'Sub-Strand',
        'Rank',
        'Action'
    ];

    keyQuestion: KeyQuestion;
    keyQuestions: KeyQuestion[] = [];
    strands: Strand[] = [];
    subStrands: SubStrand[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    // Separate arrays for the add/edit form cascading dropdowns
    formStrands: Strand[] = [];
    formSubStrands: SubStrand[] = [];
    formSubjects: Subject[] = [];
    formLearningLevels: LearningLevel[] = [];

    selectedStrandId: any;
    selectedSubStrandId: any;
    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private keyQuestionSvc: KeyQuestionService,
        private subStrandSvc: SubStrandService,
        private strandSvc: StrandService,
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
                this.academicYears = academicYears.sort((a, b) => a.rank - b.rank);
                this.educationLevels = educationLevels.sort((a, b) => a.rank - b.rank);

                let topAcademicYear = this.academicYears.find((y) => y.status == true);
                let topCurriculum = this.curricula[0];

                this.learninglevelSvc
                    .getLearningLevelsByCurriculum(parseInt(topCurriculum.id))
                    .subscribe({
                        next: (learnLvls) => {
                            this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
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
                                        this.searchStrands(
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

    searchStrands = (
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
        this.keyQuestionFormComp.editMode = false;
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.subStrands = this.keyQuestions = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        if (!acadYearId) return;
        let curId = this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumFilterFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.subStrands = this.keyQuestions = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        let acadYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumAcademicYearChanged = (currId: number, acadId: number) => {
        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
            next: (learnLvls) => {
                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelFilterFormChanged = (learnLvlId: number) => {
        this.subjects = this.strands = [];
        this.subStrands = this.keyQuestions = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
        let learnLvl = this.learningLevels.find((l) => parseInt(l.id) == learnLvlId);
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId').setValue(null);
        let acadYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId').value;
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
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
        this.strands = [];
        this.subStrands = this.keyQuestions = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/strands/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.strands = strands.sort((a, b) => a.rank - b.rank);
                this.subStrands = [];
                this.keyQuestions = [];
                this.selectedStrandId = null;
                this.selectedSubStrandId = null;
                if (this.strands.length > 0) {
                    this.selectedStrandId = 'all';
                    this.loadSubStrands();
                } else if (!this.firstLoad) {
                    this.toastr.info('No strands found for the selected search items!');
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    strandChanged = () => {
        this.subStrands = [];
        this.keyQuestions = [];
        this.selectedSubStrandId = null;
        if (this.selectedStrandId) {
            this.loadSubStrands();
        }
    };

    loadSubStrands = () => {
        if (this.selectedStrandId === 'all') {
            let requests = this.strands.map((s) =>
                this.subStrandSvc.get(`/subStrands/byStrandId/${s.id}`)
            );
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.subStrands = results
                        .reduce((acc, arr) => acc.concat(arr), [])
                        .sort((a, b) => a.rank - b.rank);
                    if (this.subStrands.length > 0) {
                        this.selectedSubStrandId = 'all';
                        this.loadKeyQuestions();
                    }
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.subStrandSvc.get(`/subStrands/byStrandId/${this.selectedStrandId}`).subscribe({
                next: (subStrands) => {
                    this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
                    if (this.subStrands.length > 0) {
                        this.selectedSubStrandId = 'all';
                        this.loadKeyQuestions();
                    }
                },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    subStrandChanged = () => {
        this.keyQuestions = [];
        if (this.selectedSubStrandId) {
            this.loadKeyQuestions();
        }
    };

    loadKeyQuestions = () => {
        if (this.selectedSubStrandId === 'all') {
            let requests = this.subStrands.map((ss) =>
                this.keyQuestionSvc.get(`/keyQuestions/bySubStrandId/${ss.id}`)
            );
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.keyQuestions = results
                        .reduce((acc, arr) => acc.concat(arr), [])
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.keyQuestionSvc.get(`/keyQuestions/bySubStrandId/${this.selectedSubStrandId}`).subscribe({
                next: (keyQuestions) => {
                    this.keyQuestions = keyQuestions.sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    // ---- Add-form cascading handlers ----

    academicYearAddFormChanged = (acadYearId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        this.formSubStrands = [];
        if (!acadYearId) return;
        let curId = this.keyQuestionFormComp.keyQuestionForm.get('curriculumId')?.value;
        if (curId) {
            this.formCurriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        this.formSubStrands = [];
        let acadYearId = this.keyQuestionFormComp.keyQuestionForm.get('academicYearId')?.value;
        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.formCurriculumAcademicYearChanged(currId, acadYearId);
    };

    formCurriculumAcademicYearChanged = (currId: number, acadId: number) => {
        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
            next: (learnLvls) => {
                this.formLearningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelAddFormChanged = (learnLvlId: number) => {
        this.formSubjects = this.formStrands = [];
        this.formSubStrands = [];
        let acadYearId = this.keyQuestionFormComp.keyQuestionForm.get('academicYearId')?.value;
        if (!learnLvlId || !acadYearId) return;
        let learnLvl = this.formLearningLevels.find((l) => parseInt(l.id) == learnLvlId);
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.formSubjects = elSubjects
                        .map((es) => es.subject)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    subjectAddFormChanged = (subjectId: number) => {
        this.formStrands = [];
        this.formSubStrands = [];
        if (!subjectId) return;
        let learningLevelId = this.keyQuestionFormComp.keyQuestionForm.get('learningLevelId')?.value;
        let searchStr = `/strands/bySubjectId?learningLvlId=${learningLevelId ?? ''}&subjectId=${subjectId}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.formStrands = strands.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    strandAddFormChanged = (strandId: number) => {
        this.formSubStrands = [];
        if (!strandId) return;
        this.subStrandSvc.get(`/subStrands/byStrandId/${strandId}`).subscribe({
            next: (subStrands) => {
                this.formSubStrands = subStrands.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // ---- Edit item ----

    editItem(id: number) {
        this.keyQuestionSvc.getById(id, '/keyQuestions').subscribe(
            (res) => {
                let keyQuestionId = res.id;
                this.keyQuestion = new KeyQuestion(res);
                this.keyQuestion.id = keyQuestionId;

                // Load the sub-strand to get its strand, then the strand to get filter data
                this.subStrandSvc.getById(this.keyQuestion.subStrandId, '/subStrands').subscribe(
                    (subStrand) => {
                        this.strandSvc.getById(subStrand.strandId, '/strands').subscribe(
                            (strand) => {
                                this.keyQuestion.academicYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
                                this.keyQuestion.curriculumId = strand.curriculumId;
                                this.keyQuestion.learningLevelId = strand.learningLevelId;
                                this.keyQuestion.subjectId = strand.subjectId;
                                this.keyQuestion.strandId = subStrand.strandId;

                                // Populate form dropdowns for edit
                                this.formLearningLevels = this.learningLevels.length > 0
                                    ? this.learningLevels
                                    : [strand.learningLevel].filter(Boolean);
                                this.formSubjects = this.subjects.length > 0
                                    ? this.subjects
                                    : [strand.subject].filter(Boolean);
                                this.formStrands = this.strands.length > 0
                                    ? this.strands
                                    : [strand].filter(Boolean);
                                this.formSubStrands = this.subStrands.length > 0
                                    ? this.subStrands
                                    : [subStrand].filter(Boolean);

                                this.keyQuestionFormComp.setFormControls(this.keyQuestion);
                                this.keyQuestionFormComp.editMode = true;
                                this.keyQuestionFormComp.keyQuestion = this.keyQuestion;
                                this.tableButton.onClick();
                            },
                            (err) => this.toastr.error(err.error)
                        );
                    },
                    (err) => this.toastr.error(err.error)
                );
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(keyQuestion: KeyQuestion) {
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
                this.keyQuestionSvc
                    .delete('/keyQuestions', parseInt(keyQuestion.id))
                    .subscribe(
                        (res) => {
                            this.loadKeyQuestions();
                            this.toastr.success('Record deleted successfully!');
                        },
                        (err) => {
                            this.toastr.error(err.error?.message || err.message || 'Error deleting record.');
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    resetForm = () => {
        this.keyQuestionFormComp.editMode = false;
        this.keyQuestionFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addKeyQuestion = (keyQuestion: KeyQuestion) => {
        Swal.fire({
            title: `${this.keyQuestionFormComp.editMode ? 'Update' : 'Add'} key question?`,
            text: `Confirm if you want to ${
                this.keyQuestionFormComp.editMode ? 'update' : 'add'
            } key question.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.keyQuestionFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new KeyQuestion(keyQuestion);
                if (this.keyQuestionFormComp.editMode) app.id = keyQuestion.id;
                let reqToProcess = this.keyQuestionFormComp.editMode
                    ? this.keyQuestionSvc.update('/keyQuestions', app)
                    : this.keyQuestionSvc.create('/keyQuestions', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.keyQuestionFormComp.editMode = false;
                        this.toastr.success('Key question saved successfully');
                        this.closeButton.nativeElement.click();

                        // Set the main filter to match what was just saved
                        let acadYearId = keyQuestion.academicYearId;
                        let currId = keyQuestion.curriculumId;
                        let llId = keyQuestion.learningLevelId;
                        let subjId = keyQuestion.subjectId;
                        let strandId = keyQuestion.strandId;
                        let subStrandId = keyQuestion.subStrandId;

                        // Update the filter form values
                        let cysPass = new SchoolSoftFilter();
                        cysPass.academicYearId = acadYearId;
                        cysPass.curriculumId = currId;
                        cysPass.learningLevelId = llId;
                        cysPass.subjectId = subjId;

                        // Reload learning levels and subjects for the filter
                        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
                            next: (learnLvls) => {
                                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
                                let learnLvl = this.learningLevels.find((l) => parseInt(l.id) == llId);
                                if (learnLvl) {
                                    this.educationlevelSubjectsSvc
                                        .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
                                        .subscribe({
                                            next: (elSubjects) => {
                                                this.subjects = elSubjects
                                                    .map((es) => es.subject)
                                                    .sort((a, b) => a.rank - b.rank);

                                                // Set the filter form and trigger search
                                                this.ssFFormComp.setFormControls(cysPass);

                                                // Load strands for this subject
                                                let searchStr = `/strands/bySubjectId?academicYearId=${acadYearId ?? ''}&learningLvlId=${llId ?? ''}&subjectId=${subjId ?? ''}`;
                                                this.strandSvc.get(searchStr).subscribe({
                                                    next: (strands) => {
                                                        this.strands = strands.sort((a, b) => a.rank - b.rank);
                                                        this.selectedStrandId = strandId;

                                                        // Load sub-strands for selected strand
                                                        this.subStrandSvc.get(`/subStrands/byStrandId/${strandId}`).subscribe({
                                                            next: (subStrands) => {
                                                                this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
                                                                this.selectedSubStrandId = subStrandId;
                                                                this.loadKeyQuestions();
                                                            },
                                                            error: (err) => this.toastr.error(err.error)
                                                        });
                                                    },
                                                    error: (err) => this.toastr.error(err.error)
                                                });
                                            },
                                            error: (err) => this.toastr.error(err.error)
                                        });
                                }
                            },
                            error: (err) => this.toastr.error(err.error)
                        });

                        this.keyQuestionFormComp.refreshItems();
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
