import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {LessonAllocationAddFormComponent} from './lesson-allocation-add-form/lesson-allocation-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {LessonAllocation} from '../../models/lesson-allocation';
import {LessonAllocationService} from '../../services/lesson-allocation.service';
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
    selector: 'app-lesson-allocations',
    templateUrl: './lesson-allocations.component.html',
    styleUrl: './lesson-allocations.component.scss'
})
export class LessonAllocationsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(LessonAllocationAddFormComponent)
    lessonAllocationFormComp: LessonAllocationAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'lessonAllocation';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/lesson-allocations'], title: 'Lesson Allocations'}
    ];
    dashboardTitle = 'CBE Assessments: Lesson Allocations';
    tableTitle: string = ' Lesson Allocations list';
    tableHeaders: string[] = [
        'Ref#',
        'Subject',
        'Learning Level',
        'Lessons/Week',
        'Description',
        'Action'
    ];

    lessonAllocation: LessonAllocation;
    lessonAllocations: LessonAllocation[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    formSubjects: Subject[] = [];
    formLearningLevels: LearningLevel[] = [];

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private lessonAllocationSvc: LessonAllocationService,
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
                                        this.searchAllocations(
                                            parseInt(topAcademicYear.id),
                                            parseInt(topCurriculum.id),
                                            parseInt(topLearningLevel.id),
                                            parseInt(this.subjects[0]?.id)
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

    searchAllocations = (
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
        this.lessonAllocationFormComp.editMode = false;
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = [];
        this.lessonAllocations = [];
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        if (!acadYearId) return;
        let curId = this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumFilterFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = [];
        this.lessonAllocations = [];
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
        this.subjects = [];
        this.lessonAllocations = [];
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
        this.lessonAllocations = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        if (!cys.subjectId) {
            if (!this.firstLoad) this.toastr.info('Please select a subject.');
            this.firstLoad = false;
            return;
        }
        this.lessonAllocationSvc.get(`/lessonAllocations/bySubjectId/${cys.subjectId}`).subscribe({
            next: (allocations) => {
                // Filter by learning level if selected
                if (cys.learningLevelId) {
                    this.lessonAllocations = allocations.filter(
                        (a) => a.learningLevelId == cys.learningLevelId
                    );
                } else {
                    this.lessonAllocations = allocations;
                }
                if (this.lessonAllocations.length === 0 && !this.firstLoad) {
                    this.toastr.info('No lesson allocations found for the selected filters.');
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadLessonAllocations = () => {
        let subjectId = this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.value;
        let learningLevelId = this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.value;
        if (!subjectId) return;
        this.lessonAllocationSvc.get(`/lessonAllocations/bySubjectId/${subjectId}`).subscribe({
            next: (allocations) => {
                if (learningLevelId) {
                    this.lessonAllocations = allocations.filter(
                        (a) => a.learningLevelId == learningLevelId
                    );
                } else {
                    this.lessonAllocations = allocations;
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // ---- Add-form cascading handlers ----

    academicYearAddFormChanged = (acadYearId: number) => {
        this.formSubjects = this.formLearningLevels = [];
        if (!acadYearId) return;
        let curId = this.lessonAllocationFormComp.lessonAllocationForm.get('curriculumId')?.value;
        if (curId) {
            this.formCurriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.formSubjects = this.formLearningLevels = [];
        let acadYearId = this.lessonAllocationFormComp.lessonAllocationForm.get('academicYearId')?.value;
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
        this.formSubjects = [];
        let acadYearId = this.lessonAllocationFormComp.lessonAllocationForm.get('academicYearId')?.value;
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

    // ---- Edit item ----

    editItem(id: number) {
        this.lessonAllocationSvc.getById(id, '/lessonAllocations').subscribe(
            (res) => {
                let lessonAllocationId = res.id;
                this.lessonAllocation = new LessonAllocation(res);
                this.lessonAllocation.id = lessonAllocationId;

                this.lessonAllocation.academicYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
                this.lessonAllocation.curriculumId = this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;

                this.formLearningLevels = this.learningLevels.length > 0
                    ? this.learningLevels
                    : [];
                this.formSubjects = this.subjects.length > 0
                    ? this.subjects
                    : [];

                this.lessonAllocationFormComp.setFormControls(this.lessonAllocation);
                this.lessonAllocationFormComp.editMode = true;
                this.lessonAllocationFormComp.lessonAllocation = this.lessonAllocation;
                this.tableButton.onClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(lessonAllocation: LessonAllocation) {
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
                this.lessonAllocationSvc
                    .delete('/lessonAllocations', parseInt(lessonAllocation.id))
                    .subscribe(
                        (res) => {
                            this.loadLessonAllocations();
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
        this.lessonAllocationFormComp.editMode = false;
        this.lessonAllocationFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addLessonAllocation = (lessonAllocation: LessonAllocation) => {
        Swal.fire({
            title: `${this.lessonAllocationFormComp.editMode ? 'Update' : 'Add'} lesson allocation?`,
            text: `Confirm if you want to ${
                this.lessonAllocationFormComp.editMode ? 'update' : 'add'
            } lesson allocation.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.lessonAllocationFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new LessonAllocation(lessonAllocation);
                if (this.lessonAllocationFormComp.editMode) app.id = lessonAllocation.id;
                let reqToProcess = this.lessonAllocationFormComp.editMode
                    ? this.lessonAllocationSvc.update('/lessonAllocations', app)
                    : this.lessonAllocationSvc.create('/lessonAllocations', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.lessonAllocationFormComp.editMode = false;
                        this.toastr.success('Lesson allocation saved successfully');
                        this.closeButton.nativeElement.click();

                        let acadYearId = lessonAllocation.academicYearId;
                        let currId = lessonAllocation.curriculumId;
                        let llId = lessonAllocation.learningLevelId;
                        let subjId = lessonAllocation.subjectId;

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
                                                this.loadLessonAllocations();
                                            },
                                            error: (err) => this.toastr.error(err.error)
                                        });
                                }
                            },
                            error: (err) => this.toastr.error(err.error)
                        });

                        this.lessonAllocationFormComp.refreshItems();
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
