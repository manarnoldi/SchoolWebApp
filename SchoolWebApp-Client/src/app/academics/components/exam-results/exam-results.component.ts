import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {ExamName} from '@/academics/models/exam-name';
import {ExamResult} from '@/academics/models/exam-result';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {ExamNamesService} from '@/academics/services/exam-names.service';
import {ExamResultsService} from '@/academics/services/exam-results.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {ExamsService} from '@/academics/services/exams.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SessionsService} from '@/class/services/sessions.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-exam-results',
    templateUrl: './exam-results.component.html',
    styleUrl: './exam-results.component.scss'
})
export class ExamResultsComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    examResults: ExamResult[] = [];
    examTypes: ExamType[] = [];
    examNames: ExamName[] = [];
    subjects: Subject[] = [];

    currentExam: Exam;

    schoolClasses: SchoolClass[] = [];
    sessions: Session[] = [];
    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    educationLevels: EducationLevel[] = [];
    isLoading: boolean = false;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/exam-results'], title: 'Academics: Exam results'}
    ];
    dashboardTitle = 'Academics: Exam results';

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private curriculaSvc: CurriculumService,
        private subjectsSvc: SubjectsService,
        private schoolClassesSvc: SchoolClassesService,
        private educLevelsSvc: EducationLevelService,
        private examsSvc: ExamsService,
        private examTypesSvc: ExamTypesService,
        private examNamesSvc: ExamNamesService,
        private examResultsSvc: ExamResultsService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    get missingMarksPresent() {
        return this.examResults.filter((er) => !er.id).length;
    }

    refreshItems() {
        let acadYearsReq = this.academicYearSvc.get('/academicYears');
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let examTypesReq = this.examTypesSvc.get('/examTypes');
        forkJoin([acadYearsReq, curriculaReq, examTypesReq]).subscribe({
            next: ([academicYears, curricula, examTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.isLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }

    academicYearChanged = (acadYearId: number) => {
        this.examResults = [];
        this.currentExam = null;
        if (acadYearId) {
            this.curriculumYearChanged();
        }
    };

    curriculumChanged = (curriculumId: number) => {
        this.examResults = [];
        this.currentExam = null;
        if (curriculumId) {
            this.curriculumYearChanged();
        }
    };

    educationLevelChanged = (levelId: number) => {
        this.schoolClasses = [];
        this.subjects = [];
        this.examResults = [];
        this.currentExam = null;
        this.resetFormControls(false, false, true, true, true, true);

        let acadYearId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                'academicYearId'
            ).value;

        if (levelId && acadYearId) {
            let schoolClassesReq =
                this.schoolClassesSvc.getByEducationLevelandYear(
                    levelId,
                    acadYearId
                );
            let subjectsReq = this.subjectsSvc.getSubjectsByEducationLevelYear(
                levelId,
                acadYearId
            );

            forkJoin([schoolClassesReq, subjectsReq]).subscribe({
                next: ([schoolClasses, subjects]) => {
                    this.schoolClasses = schoolClasses.sort(
                        (a, b) => a.rank - b.rank
                    );
                    this.subjects = subjects.sort((a, b) => a.rank - b.rank);
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
        }
    };

    curriculumYearChanged = () => {
        this.sessions = [];
        this.educationLevels = [];
        this.examResults = [];
        this.currentExam = null;
        this.resetFormControls(true, true, true, true, true, true);

        let curriculumId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                'curriculumId'
            ).value;
        let acadYearId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                'academicYearId'
            ).value;

        if (curriculumId && acadYearId) {
            let sessionsReq = this.sessionsSvc.getByCurriculumYear(
                curriculumId,
                acadYearId
            );
            let educLevelReq =
                this.educLevelsSvc.educationLevelsByCurriculum(curriculumId);

            forkJoin([sessionsReq, educLevelReq]).subscribe({
                next: ([sessions, educLevels]) => {
                    this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                    this.educationLevels = educLevels.sort(
                        (a, b) => a.rank - b.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
        }
    };

    sessionChanged = (sessionId: number) => {
        this.resetFormControls(false, true, true, true, true, true);
        this.examResults = [];
        this.currentExam = null;
    };

    schoolClassChanged = (schoolClassId: number) => {
        this.resetFormControls(false, false, false, true, true, true);
        this.examResults = [];
        this.currentExam = null;
    };

    subjectChanged = (subjectId: number) => {
        this.resetFormControls(false, false, false, false, true, true);
        this.examResults = [];
        this.currentExam = null;
    };

    examTypeChanged = (examTypeId: number) => {
        this.examResults = [];
        this.currentExam = null;
        this.resetFormControls(false, false, false, false, false, true);

        this.examNamesSvc.getByExamTypeId(examTypeId).subscribe({
            next: (examNames) => {
                this.examNames = examNames;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    examNameChanged = (examNameId: number) => {
        this.currentExam = null;
        this.examResults = [];
    };

    deleteExamResult = (examR: ExamResult) => {
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
                this.examResultsSvc
                    .delete('/examResults', parseInt(examR.id))
                    .subscribe(
                        (res) => {
                            this.examResultsSvc
                                .loadExamResults(this.currentExam, false)
                                .subscribe({
                                    next: (examRes) => {
                                        this.examResults = examRes;
                                        this.toastr.success(
                                            'Exam result deleted successfully'
                                        );
                                    },
                                    error: (err) => {
                                        this.toastr.error(err.error);
                                    }
                                });
                        },
                        (err) => {
                            this.toastr.error(err);
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

    onButtonSearchClick = (ssf: SchoolSoftFilter) => {
        this.examResults = [];
        if (!ssf?.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!ssf.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!ssf.sessionId)
            this.toastr.info('Select session before searching!');
        else if (!ssf.educationLevelId)
            this.toastr.info('Select education level before searching!');
        else if (!ssf.schoolClassId)
            this.toastr.info('Select class before searching!');
        else if (!ssf.subjectId)
            this.toastr.info('Select subject before searching!');
        else if (!ssf.examTypeId)
            this.toastr.info('Select exam type before searching!');
        else if (!ssf.examNameId)
            this.toastr.info('Select exam name before searching!');
        else {
            this.examsSvc.getExamsBySearch(ssf).subscribe({
                next: (exams) => {
                    if (!exams || exams.length <= 0) {
                        this.toastr.error(
                            'There are no exams registered under the selections!'
                        );
                        return;
                    }

                    this.currentExam = exams[0];
                    this.examResultsSvc
                        .loadExamResults(this.currentExam, false)
                        .subscribe({
                            next: (examRes) => {
                                this.examResults = examRes;
                                if (this.examResults.length <= 0) {
                                    this.toastr.info(
                                        'No record found with the selection!'
                                    );
                                }
                            },
                            error: (err) => {
                                this.toastr.error(err.error);
                            }
                        });
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
        }
    };

    submitExamResults = () => {
        if (!this.examResults || this.examResults.length <= 0) {
            this.toastr.error(
                'There are no exam results on the list. Do the selections and load the results.'
            );
            return;
        }

        Swal.fire({
            title: `Submit exam results?`,
            text: `Confirm if you want to submit exam results.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Submit`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.examResults.map((e) => {
                    e.exam = null;
                    e.student = null;
                    e.id = e.id == null ? '0' : e.id;
                });
                this.examResultsSvc
                    .createBatch('/examResults/batch', this.examResults)
                    .subscribe({
                        next: (res) => {
                            this.examResultsSvc
                                .loadExamResults(this.currentExam, false)
                                .subscribe({
                                    next: (examRes) => {
                                        this.examResults = examRes;
                                        this.toastr.success(
                                            'Exam results saved successfully'
                                        );
                                    },
                                    error: (err) => {
                                        this.toastr.error(err.error);
                                    }
                                });
                        },
                        error: (err) => {
                            this.toastr.error(err.error?.message);
                        }
                    });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

    resetFormControls = (
        sessionIdReset: boolean,
        educationLevelIdReset: boolean,
        schoolClassIdReset: boolean,
        subjectIdReset: boolean,
        examTypeIdReset: boolean,
        examNameIdReset: boolean
    ) => {
        if (sessionIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('sessionId')
                .reset();
        if (educationLevelIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('educationLevelId')
                .reset();
        if (schoolClassIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('schoolClassId')
                .reset();
        if (subjectIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('subjectId')
                .reset();
        if (examTypeIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('examTypeId')
                .reset();
        if (examNameIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('examNameId')
                .reset();
    };
}
