import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {LearningExperienceAddFormComponent} from './learning-experience-add-form/learning-experience-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {LearningExperience} from '../../models/learning-experience';
import {LearningExperienceService} from '../../services/learning-experience.service';
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
    selector: 'app-learning-experiences',
    templateUrl: './learning-experiences.component.html',
    styleUrl: './learning-experiences.component.scss'
})
export class LearningExperiencesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(LearningExperienceAddFormComponent)
    learningExperienceFormComp: LearningExperienceAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'learningExperience';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/learning-experiences'], title: 'Learning Experiences'}
    ];
    dashboardTitle = 'CBE Assessments: Learning Experiences';
    tableTitle: string = ' Learning Experiences list';
    tableHeaders: string[] = [
        'Ref#',
        'Learning Experience',
        'Sub-Strand',
        'Rank',
        'Action'
    ];

    learningExperience: LearningExperience;
    learningExperiences: LearningExperience[] = [];
    strands: Strand[] = [];
    subStrands: SubStrand[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    formStrands: Strand[] = [];
    formSubStrands: SubStrand[] = [];
    formSubjects: Subject[] = [];
    formLearningLevels: LearningLevel[] = [];

    selectedStrandId: any;
    selectedSubStrandId: any;
    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private learningExperienceSvc: LearningExperienceService,
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
        this.learningExperienceFormComp.editMode = false;
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.subStrands = this.learningExperiences = [];
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
        this.subStrands = this.learningExperiences = [];
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
        this.subStrands = this.learningExperiences = [];
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
        this.subStrands = this.learningExperiences = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/strands/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.strands = strands.sort((a, b) => a.rank - b.rank);
                this.subStrands = [];
                this.learningExperiences = [];
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
        this.learningExperiences = [];
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
                        this.loadLearningExperiences();
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
                        this.loadLearningExperiences();
                    }
                },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    subStrandChanged = () => {
        this.learningExperiences = [];
        if (this.selectedSubStrandId) {
            this.loadLearningExperiences();
        }
    };

    loadLearningExperiences = () => {
        if (this.selectedSubStrandId === 'all') {
            let requests = this.subStrands.map((ss) =>
                this.learningExperienceSvc.get(`/learningExperiences/bySubStrandId/${ss.id}`)
            );
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.learningExperiences = results
                        .reduce((acc, arr) => acc.concat(arr), [])
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.learningExperienceSvc.get(`/learningExperiences/bySubStrandId/${this.selectedSubStrandId}`).subscribe({
                next: (learningExperiences) => {
                    this.learningExperiences = learningExperiences.sort((a, b) => a.rank - b.rank);
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
        let curId = this.learningExperienceFormComp.learningExperienceForm.get('curriculumId')?.value;
        if (curId) {
            this.formCurriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        this.formSubStrands = [];
        let acadYearId = this.learningExperienceFormComp.learningExperienceForm.get('academicYearId')?.value;
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
        let acadYearId = this.learningExperienceFormComp.learningExperienceForm.get('academicYearId')?.value;
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
        let learningLevelId = this.learningExperienceFormComp.learningExperienceForm.get('learningLevelId')?.value;
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
        this.learningExperienceSvc.getById(id, '/learningExperiences').subscribe(
            (res) => {
                let learningExperienceId = res.id;
                this.learningExperience = new LearningExperience(res);
                this.learningExperience.id = learningExperienceId;

                this.subStrandSvc.getById(this.learningExperience.subStrandId, '/subStrands').subscribe(
                    (subStrand) => {
                        this.strandSvc.getById(subStrand.strandId, '/strands').subscribe(
                            (strand) => {
                                this.learningExperience.academicYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
                                this.learningExperience.curriculumId = strand.curriculumId;
                                this.learningExperience.learningLevelId = strand.learningLevelId;
                                this.learningExperience.subjectId = strand.subjectId;
                                this.learningExperience.strandId = subStrand.strandId;

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

                                this.learningExperienceFormComp.setFormControls(this.learningExperience);
                                this.learningExperienceFormComp.editMode = true;
                                this.learningExperienceFormComp.learningExperience = this.learningExperience;
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

    deleteItem(learningExperience: LearningExperience) {
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
                this.learningExperienceSvc
                    .delete('/learningExperiences', parseInt(learningExperience.id))
                    .subscribe(
                        (res) => {
                            this.loadLearningExperiences();
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
        this.learningExperienceFormComp.editMode = false;
        this.learningExperienceFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addLearningExperience = (learningExperience: LearningExperience) => {
        Swal.fire({
            title: `${this.learningExperienceFormComp.editMode ? 'Update' : 'Add'} learning experience?`,
            text: `Confirm if you want to ${
                this.learningExperienceFormComp.editMode ? 'update' : 'add'
            } learning experience.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.learningExperienceFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new LearningExperience(learningExperience);
                if (this.learningExperienceFormComp.editMode) app.id = learningExperience.id;
                let reqToProcess = this.learningExperienceFormComp.editMode
                    ? this.learningExperienceSvc.update('/learningExperiences', app)
                    : this.learningExperienceSvc.create('/learningExperiences', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.learningExperienceFormComp.editMode = false;
                        this.toastr.success('Learning experience saved successfully');
                        this.closeButton.nativeElement.click();

                        let acadYearId = learningExperience.academicYearId;
                        let currId = learningExperience.curriculumId;
                        let llId = learningExperience.learningLevelId;
                        let subjId = learningExperience.subjectId;
                        let strandId = learningExperience.strandId;
                        let subStrandId = learningExperience.subStrandId;

                        let cysPass = new SchoolSoftFilter();
                        cysPass.academicYearId = acadYearId;
                        cysPass.curriculumId = currId;
                        cysPass.learningLevelId = llId;
                        cysPass.subjectId = subjId;

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

                                                this.ssFFormComp.setFormControls(cysPass);

                                                let searchStr = `/strands/bySubjectId?academicYearId=${acadYearId ?? ''}&learningLvlId=${llId ?? ''}&subjectId=${subjId ?? ''}`;
                                                this.strandSvc.get(searchStr).subscribe({
                                                    next: (strands) => {
                                                        this.strands = strands.sort((a, b) => a.rank - b.rank);
                                                        this.selectedStrandId = strandId;

                                                        this.subStrandSvc.get(`/subStrands/byStrandId/${strandId}`).subscribe({
                                                            next: (subStrands) => {
                                                                this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
                                                                this.selectedSubStrandId = subStrandId;
                                                                this.loadLearningExperiences();
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

                        this.learningExperienceFormComp.refreshItems();
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
