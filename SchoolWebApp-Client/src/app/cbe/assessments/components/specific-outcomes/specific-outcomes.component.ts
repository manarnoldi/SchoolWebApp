import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {SpecificOutcomeAddFormComponent} from './specific-outcome-add-form/specific-outcome-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SpecificOutcome} from '../../models/specific-outcome';
import {SpecificOutcomeService} from '../../services/specific-outcome.service';
import {SubStrand} from '../../models/sub-strand';
import {SubStrandService} from '../../services/sub-strand.service';
import {Strand} from '../../models/strand';
import {StrandService} from '../../services/strand.service';
import {BroadOutcome} from '../../models/broad-outcome';
import {BroadOutcomeService} from '../../services/broad-outcome.service';
import {GeneralOutcome} from '../../models/general-outcome';
import {GeneralOutcomeService} from '../../services/general-outcome.service';
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
import {SessionsService} from '@/class/services/sessions.service';

@Component({
    selector: 'app-specific-outcomes',
    templateUrl: './specific-outcomes.component.html',
    styleUrl: './specific-outcomes.component.scss'
})
export class SpecificOutcomesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SpecificOutcomeAddFormComponent)
    specificOutcomeFormComp: SpecificOutcomeAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'specificOutcome';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/specific-outcomes'], title: 'Specific Outcomes'}
    ];
    dashboardTitle = 'CBE Assessments: Specific Outcomes';
    tableTitle: string = ' Specific Outcomes list';
    tableHeaders: string[] = [
        'Ref#',
        'Specific Outcome',
        'Sub-Strand',
        'General Outcome',
        'Broad Outcome',
        'Rank',
        'Action'
    ];

    specificOutcome: SpecificOutcome;
    specificOutcomes: SpecificOutcome[] = [];
    strands: Strand[] = [];
    subStrands: SubStrand[] = [];
    broadOutcomes: BroadOutcome[] = [];
    generalOutcomes: GeneralOutcome[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];
    sessions: any[] = [];

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
        private specificOutcomeSvc: SpecificOutcomeService,
        private subStrandSvc: SubStrandService,
        private strandSvc: StrandService,
        private broadOutcomeSvc: BroadOutcomeService,
        private generalOutcomeSvc: GeneralOutcomeService,
        private subjectsSvc: SubjectsService,
        private curriculaSvc: CurriculumService,
        private learninglevelSvc: LearningLevelsService,
        private educationlevelSvc: EducationLevelService,
        private educationlevelSubjectsSvc: EducationLevelSubjectService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService
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
        let broadOutcomesReq = this.broadOutcomeSvc.get('/broadOutcomes');
        let generalOutcomesReq = this.generalOutcomeSvc.get('/generalOutcomes');
        let sessionsReq = this.sessionsSvc.get('/sessions');

        forkJoin([
            curriculaReq,
            academicYearsReq,
            educationLevelsReq,
            broadOutcomesReq,
            generalOutcomesReq,
            sessionsReq
        ]).subscribe({
            next: ([curricula, academicYears, educationLevels, broadOutcomes, generalOutcomes, sessions]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => a.rank - b.rank);
                this.educationLevels = educationLevels.sort((a, b) => a.rank - b.rank);
                this.broadOutcomes = broadOutcomes.sort((a, b) => a.rank - b.rank);
                this.generalOutcomes = generalOutcomes.sort((a, b) => a.rank - b.rank);

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
        this.specificOutcomeFormComp.editMode = false;
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.subStrands = this.specificOutcomes = [];
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
        this.subStrands = this.specificOutcomes = [];
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
        this.subStrands = this.specificOutcomes = [];
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
        this.subStrands = this.specificOutcomes = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/strands/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.strands = strands.sort((a, b) => a.rank - b.rank);
                this.subStrands = [];
                this.specificOutcomes = [];
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
        this.specificOutcomes = [];
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
                        this.loadSpecificOutcomes();
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
                        this.loadSpecificOutcomes();
                    }
                },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    subStrandChanged = () => {
        this.specificOutcomes = [];
        if (this.selectedSubStrandId) {
            this.loadSpecificOutcomes();
        }
    };

    loadSpecificOutcomes = () => {
        if (this.selectedSubStrandId === 'all') {
            let requests = this.subStrands.map((ss) =>
                this.specificOutcomeSvc.get(`/specificOutcomes/bySubStrandId/${ss.id}`)
            );
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.specificOutcomes = results
                        .reduce((acc, arr) => acc.concat(arr), [])
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.specificOutcomeSvc.get(`/specificOutcomes/bySubStrandId/${this.selectedSubStrandId}`).subscribe({
                next: (specificOutcomes) => {
                    this.specificOutcomes = specificOutcomes.sort((a, b) => a.rank - b.rank);
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
        let curId = this.specificOutcomeFormComp.specificOutcomeForm.get('curriculumId')?.value;
        if (curId) {
            this.formCurriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        this.formSubStrands = [];
        let acadYearId = this.specificOutcomeFormComp.specificOutcomeForm.get('academicYearId')?.value;
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
        let acadYearId = this.specificOutcomeFormComp.specificOutcomeForm.get('academicYearId')?.value;
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
        let learningLevelId = this.specificOutcomeFormComp.specificOutcomeForm.get('learningLevelId')?.value;
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
        this.specificOutcomeSvc.getById(id, '/specificOutcomes').subscribe(
            (res) => {
                let specificOutcomeId = res.id;
                this.specificOutcome = new SpecificOutcome(res);
                this.specificOutcome.id = specificOutcomeId;

                // Load the sub-strand to get its strand, then the strand to get filter data
                this.subStrandSvc.getById(this.specificOutcome.subStrandId, '/subStrands').subscribe(
                    (subStrand) => {
                        this.strandSvc.getById(subStrand.strandId, '/strands').subscribe(
                            (strand) => {
                                this.specificOutcome.academicYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
                                this.specificOutcome.curriculumId = strand.curriculumId;
                                this.specificOutcome.learningLevelId = strand.learningLevelId;
                                this.specificOutcome.subjectId = strand.subjectId;
                                this.specificOutcome.strandId = subStrand.strandId;

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

                                this.specificOutcomeFormComp.setFormControls(this.specificOutcome);
                                this.specificOutcomeFormComp.editMode = true;
                                this.specificOutcomeFormComp.specificOutcome = this.specificOutcome;
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

    deleteItem(specificOutcome: SpecificOutcome) {
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
                this.specificOutcomeSvc
                    .delete('/specificOutcomes', parseInt(specificOutcome.id))
                    .subscribe(
                        (res) => {
                            this.loadSpecificOutcomes();
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
        this.specificOutcomeFormComp.editMode = false;
        this.specificOutcomeFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSpecificOutcome = (specificOutcome: SpecificOutcome) => {
        Swal.fire({
            title: `${this.specificOutcomeFormComp.editMode ? 'Update' : 'Add'} specific outcome?`,
            text: `Confirm if you want to ${
                this.specificOutcomeFormComp.editMode ? 'update' : 'add'
            } specific outcome.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.specificOutcomeFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new SpecificOutcome(specificOutcome);
                if (this.specificOutcomeFormComp.editMode) app.id = specificOutcome.id;
                let reqToProcess = this.specificOutcomeFormComp.editMode
                    ? this.specificOutcomeSvc.update('/specificOutcomes', app)
                    : this.specificOutcomeSvc.create('/specificOutcomes', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.specificOutcomeFormComp.editMode = false;
                        this.toastr.success('Specific outcome saved successfully');
                        this.closeButton.nativeElement.click();

                        // Set the main filter to match what was just saved
                        let acadYearId = specificOutcome.academicYearId;
                        let currId = specificOutcome.curriculumId;
                        let llId = specificOutcome.learningLevelId;
                        let subjId = specificOutcome.subjectId;
                        let strandId = specificOutcome.strandId;
                        let subStrandId = specificOutcome.subStrandId;

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
                                                                this.loadSpecificOutcomes();
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

                        this.specificOutcomeFormComp.refreshItems();
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
