import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumYear} from '@/academics/models/curriculum-year';
import {Exam} from '@/academics/models/exam';
import {ExamResult} from '@/academics/models/exam-result';
import {ExamSearch} from '@/academics/models/exam-search';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {ExamResultsService} from '@/academics/services/exam-results.service';
import {ExamsService} from '@/academics/services/exams.service';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-exam-results',
    templateUrl: './exam-results.component.html',
    styleUrl: './exam-results.component.scss'
})
export class ExamResultsComponent implements OnInit {
    examResults: ExamResult[] = [];
    examTypes: ExamType[] = [];
    subjects: Subject[] = [];
    exams: Exam[] = [];
    exam: Exam;

    educLevelId: number;

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
        private examsSvc: ExamsService,
        private examResultsSvc: ExamResultsService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        this.examsSvc.getInitialItems().subscribe({
            next: ([
                curricula,
                academicYears,
                educationLevels,
                examTypes
            ]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                this.academicYears = academicYears.sort(
                    (a, b) => a.rank - b.rank
                );
                this.isLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }

    examTypeChanged = (es: ExamSearch) => {
        this.exam = {} as Exam;
        this.exams = [];
        this.examResults = [];
        this.examsSvc.getExamsBySearch(es).subscribe({
            next: (exams) => {
                this.exams = exams;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    educationLevelYearChanged = (ely: EducationLevelYear) => {
        this.educLevelId = ely.educationLevelId;
        this.subjects = [];
        this.exam = {} as Exam;
        this.schoolClasses = [];
        let educationLevelSubjectsReq =
            this.examsSvc.getSubjectsByEducationLevelYear(ely);
        let schoolClassReq =
            this.examsSvc.getSchoolClassesByEducationLevelYear(ely);

        forkJoin([educationLevelSubjectsReq, schoolClassReq]).subscribe({
            next: ([subjects, schoolClasses]) => {
                this.subjects = subjects;
                this.schoolClasses = schoolClasses;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    curriculumYearChanged = (cy: CurriculumYear) => {
        this.exams = [];
        this.exam = {} as Exam;
        this.sessions = [];
        this.examsSvc.getSessionFromCurriculumYear(cy).subscribe({
            next: (sessions) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    clearList = () => {
        this.examResults = [];
        this.exams = [];
        this.exam = {} as Exam;
    };

    examChanged = (exam: Exam) => {
        this.exam = exam;
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
                                .loadExamResults(
                                    this.exam
                                )
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

    onButtonSearchClick = () => {
        this.examResults = [];
        if (!this.exam?.schoolClass?.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!this.exam?.session?.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!this.exam?.sessionId)
            this.toastr.info('Select session before searching!');
        else if (!this.educLevelId)
            this.toastr.info('Select education level before searching!');
        else if (!this.exam?.schoolClassId)
            this.toastr.info('Select class before searching!');
        else if (!this.exam?.subjectId)
            this.toastr.info('Select subject before searching!');
        else if (!this.exam?.examTypeId)
            this.toastr.info('Select exam type before searching!');
        else if (!this.exam.id)
            this.toastr.info('Select exam name before searching!');
        else {
            this.examResultsSvc
                .loadExamResults(
                    this.exam
                )
                .subscribe({
                    next: (examRes) => {
                        this.examResults = examRes;
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        }
    };

    submitExamResults = (examR: ExamResult) => {
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
                    e.studentSubject = null;
                    e.id = e.id == null ? '0' : e.id;
                });
                this.examResultsSvc
                    .createBatch('/examResults/batch', this.examResults)
                    .subscribe({
                        next: (res) => {
                            this.examResultsSvc
                                .loadExamResults(
                                    this.exam
                                )
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
}
