import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {ExamsService} from '@/academics/services/exams.service';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {AfterViewChecked, Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {ExamAddFormComponent} from './exam-add-form/exam-add-form.component';
import {ActivatedRoute} from '@angular/router';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {SessionsService} from '@/class/services/sessions.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {ExamName} from '@/academics/models/exam-name';
import {ExamNamesService} from '@/academics/services/exam-names.service';

@Component({
    selector: 'app-exams',
    templateUrl: './exams.component.html',
    styleUrl: './exams.component.scss'
})
export class ExamsComponent implements OnInit, AfterViewChecked {
    @ViewChild(ExamAddFormComponent)
    examAddFormComponent: ExamAddFormComponent;

    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/exams'], title: 'Academics: Exams'}
    ];
    dashboardTitle = 'Academics: Exams';

    exams: Exam[] = [];
    subjects: Subject[] = [];
    examTypes: ExamType[] = [];
    examNames: ExamName[] = [];
    schoolClasses: SchoolClass[] = [];
    sessions: Session[] = [];
    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    educationLevels: EducationLevel[] = [];

    educationLevelId: number;
    examId: number;

    isLoading: boolean = false;
    viewInitialized = false;
    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private sessionsSvc: SessionsService,
        private subjectsSvc: SubjectsService,
        private academicYearsSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService,
        private educationLevelSvc: EducationLevelService,
        private examNamesSvc: ExamNamesService,
        private examsSvc: ExamsService,
        private route: ActivatedRoute
    ) {}

    ngAfterViewChecked(): void {
        if (this.ssFilterFormComponent && !this.viewInitialized) {
            this.viewInitialized = true;
            this.route.queryParams.subscribe((params) => {
                this.examId = parseInt(params['id']);
                this.educationLevelId = parseInt(params['eduLevelId']);
                if (
                    this.examId &&
                    this.examId > 0 &&
                    this.educationLevelId &&
                    this.educationLevelId > 0
                ) {
                    this.examsSvc.getById(this.examId, '/exams').subscribe({
                        next: (exam) => {
                            let ssf = new SchoolSoftFilter();
                            ssf.academicYearId = exam.session?.academicYearId;
                            ssf.educationLevelId = this.educationLevelId;
                            ssf.curriculumId = exam.session?.curriculumId;
                            ssf.examTypeId = exam.examTypeId;
                            ssf.schoolClassId = exam.schoolClassId;
                            ssf.sessionId = exam.sessionId;
                            ssf.subjectId = exam.subjectId;

                            let sessionFromCYReq =
                                this.sessionsSvc.getByCurriculumYear(
                                    ssf.curriculumId,
                                    ssf.academicYearId
                                );

                            let educationLevelSubjectsReq =
                                this.subjectsSvc.getSubjectsByEducationLevelYear(
                                    ssf.educationLevelId,
                                    ssf.academicYearId
                                );
                            let schoolClassReq =
                                this.schoolClassesSvc.getByEducationLevelandYear(
                                    ssf.educationLevelId,
                                    ssf.academicYearId
                                );

                            let examsSearchReq =
                                this.examsSvc.getExamsBySearch(ssf);
                            let examTypesReq =
                                this.educationLevelSvc.get('/examTypes');

                            forkJoin([
                                sessionFromCYReq,
                                educationLevelSubjectsReq,
                                schoolClassReq,
                                examsSearchReq,
                                examTypesReq
                            ]).subscribe({
                                next: ([
                                    sessions,
                                    subjects,
                                    schoolClasses,
                                    exams,
                                    examTypes
                                ]) => {
                                    this.ssFilterFormComponent.schoolSoftFilterForm.reset();
                                    this.subjects = subjects;
                                    this.schoolClasses = schoolClasses;
                                    this.sessions = sessions.sort(
                                        (a, b) => a.rank - b.rank
                                    );
                                    this.examTypes = examTypes;
                                    this.exams = exams.sort((a, b) =>
                                        a.session?.academicYear?.name.localeCompare(
                                            b.session?.academicYear?.name
                                        )
                                    );
                                    this.ssFilterFormComponent.schoolSoftFilterForm.patchValue(
                                        {
                                            curriculumId:
                                                exam.session?.curriculumId,
                                            educationLevelId:
                                                this.educationLevelId,
                                            academicYearId:
                                                exam.session?.academicYearId,
                                            sessionId: exam.sessionId,
                                            schoolClassId: exam.schoolClassId,
                                            subjectId: exam.subjectId,
                                            examTypeId: exam.examName.examTypeId
                                        }
                                    );
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
            });
        }
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');
        let educationLevelReq = this.educationLevelSvc.get('/educationLevels');
        let examTypesReq = this.educationLevelSvc.get('/examTypes');

        forkJoin([
            curriculaReq,
            academicYearsReq,
            educationLevelReq,
            examTypesReq
        ]).subscribe({
            next: ([curricula, academicYears, educationLevels, examTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.isLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }

    academicYearChanged = (acadYearId: number) => {
        this.exams = [];
        if (acadYearId) {
            this.curriculumYearChanged();
        }
    };

    curriculumChanged = (curriculumId: number) => {
        this.exams = [];
        if (curriculumId) {
            this.curriculumYearChanged();
        }
    };

    educationLevelChanged = (levelId: number) => {
        this.schoolClasses = [];
        this.subjects = [];
        this.exams = [];
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
        this.exams = [];
        this.resetFormControls(false, false, true, true, true, true);

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
                this.educationLevelSvc.educationLevelsByCurriculum(
                    curriculumId
                );

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
        this.resetFormControls(false,true,true,true,true,true);
        this.exams = [];
    };

    schoolClassChanged = (schoolClassId: number) => {
        this.resetFormControls(false,false,false,true,true,true);
        this.exams = [];
    };

    subjectChanged = (subjectId: number) => {
        this.resetFormControls(false,false,false,false,true,true);
        this.exams = [];
    };

    examTypeChanged = (examTypeId: number) => {
        this.exams = [];
    };

    onButtonSearchClick = (ssf: SchoolSoftFilter) => {
        this.exams = [];
        if (!ssf.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!ssf.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!ssf.sessionId)
            this.toastr.info('Select session before searching!');
        else if (!ssf.educationLevelId)
            this.toastr.info('Select education level before searching!');
        else if (!ssf.schoolClassId)
            this.toastr.info('Select class before searching!');
        else {
            this.educationLevelId = ssf.educationLevelId;
            this.examsSvc.getExamsBySearch(ssf).subscribe({
                next: (exams) => {
                    exams.length <= 0
                        ? this.toastr.info(
                              'No records found with the search parameters selected!'
                          )
                        : (this.exams = exams);
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
        }
    };

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete exam record?`,
            text: `Confirm if you want to delete exam record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.examsSvc.delete('/exams', id).subscribe(
                    (res) => {
                        this.refreshItems();
                        this.toastr.success(
                            'Exam record deleted successfully!'
                        );
                        this.exams.splice(
                            this.exams.findIndex((e) => e.id == id.toString()),
                            1
                        );
                    },
                    (err) => {
                        this.toastr.error(err);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

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
