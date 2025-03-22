import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumYear} from '@/academics/models/curriculum-year';
import {Exam} from '@/academics/models/exam';
import {ExamSearch} from '@/academics/models/exam-search';
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
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {
    AfterViewChecked,
    Component,
    OnInit,
    ViewChild
} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {ExamAddFormComponent} from './exam-add-form/exam-add-form.component';
import {ActivatedRoute} from '@angular/router';
import {ExamListSearchFormComponent} from './exam-list-search-form/exam-list-search-form.component';

@Component({
    selector: 'app-exams',
    templateUrl: './exams.component.html',
    styleUrl: './exams.component.scss'
})
export class ExamsComponent implements OnInit, AfterViewChecked {
    @ViewChild(ExamAddFormComponent)
    examAddFormComponent: ExamAddFormComponent;
    @ViewChild(ExamListSearchFormComponent)
    searchFormComponent: ExamListSearchFormComponent;
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/exams'], title: 'Academics: Exams'}
    ];
    dashboardTitle = 'Academics: Exams';

    exams: Exam[] = [];
    subjects: Subject[] = [];
    examTypes: ExamType[] = [];
    schoolClasses: SchoolClass[] = [];
    sessions: Session[] = [];
    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    educationLevels: EducationLevel[] = [];
    eduLevelId: number;
    examId: number;
    isLoading: boolean = false;
    viewInitialized = false;
    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private educationLevelSvc: EducationLevelService,
        private examsSvc: ExamsService,
        private route: ActivatedRoute
    ) {}

    ngAfterViewChecked(): void {
        if (this.searchFormComponent && !this.viewInitialized) {
            this.viewInitialized = true;
            this.route.queryParams.subscribe((params) => {
                this.examId = parseInt(params['id']);
                this.eduLevelId = parseInt(params['eduLevelId']);
                if (
                    this.examId &&
                    this.examId > 0 &&
                    this.eduLevelId &&
                    this.eduLevelId > 0
                ) {
                    this.examsSvc.getById(this.examId, '/exams').subscribe({
                        next: (exam) => {
                            let cuyear = new CurriculumYear();
                            cuyear.academicYearId =
                                exam.session?.academicYearId;
                            cuyear.curriculumId = exam.session?.curriculumId;

                            let ely = new EducationLevelYear();
                            ely.academicYearId = exam.session?.academicYearId;
                            ely.educationLevelId = this.eduLevelId;

                            let es = new ExamSearch();
                            es.academicYearId = exam.session?.academicYearId;
                            es.educationLevelId = this.eduLevelId;
                            es.curriculumId = exam.session?.curriculumId;
                            es.examTypeId = exam.examTypeId;
                            es.schoolClassId = exam.schoolClassId;
                            es.sessionId = exam.sessionId;
                            es.subjectId = exam.subjectId;

                            let sessionFromCYReq =
                                this.examsSvc.getSessionFromCurriculumYear(cuyear);
                            let educationLevelSubjectsReq =
                                this.examsSvc.getSubjectsByEducationLevelYear(ely);
                            let schoolClassReq =
                                this.examsSvc.getSchoolClassesByEducationLevelYear(ely);
                            let examsSearchReq = this.examsSvc.getExamsBySearch(es);

                            forkJoin([
                                sessionFromCYReq,
                                educationLevelSubjectsReq,
                                schoolClassReq,
                                examsSearchReq
                            ]).subscribe({
                                next: ([
                                    sessions,
                                    subjects,
                                    schoolClasses,
                                    exams
                                ]) => {
                                    this.subjects = subjects;
                                    this.schoolClasses = schoolClasses;
                                    this.sessions = sessions.sort(
                                        (a, b) => a.rank - b.rank
                                    );
                                    this.exams = exams.sort((a, b) =>
                                        a.session?.academicYear?.name.localeCompare(
                                            b.session?.academicYear?.name
                                        )
                                    );
                                    this.searchFormComponent.examListSearchForm
                                        .get('curriculumId')
                                        .setValue(exam.session?.curriculumId);
                                    this.searchFormComponent.examListSearchForm
                                        .get('examTypeId')
                                        .setValue(exam.examTypeId);
                                    this.searchFormComponent.examListSearchForm
                                        .get('educationLevelId')
                                        .setValue(this.eduLevelId);
                                    this.searchFormComponent.examListSearchForm
                                        .get('academicYearId')
                                        .setValue(exam.session?.academicYearId);
                                    this.searchFormComponent.examListSearchForm
                                        .get('sessionId')
                                        .setValue(exam.sessionId);
                                    this.searchFormComponent.examListSearchForm
                                        .get('schoolClassId')
                                        .setValue(exam.schoolClassId);
                                    this.searchFormComponent.examListSearchForm
                                        .get('subjectId')
                                        .setValue(exam.subjectId);
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
                    (a, b) => a.rank - b.rank
                );
                this.isLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }    

    educationLevelYearChanged = (ely: EducationLevelYear) => {
        this.subjects = [];
        this.schoolClasses = [];
        let educationLevelSubjectsReq =
            this.examsSvc.getSubjectsByEducationLevelYear(ely);
        let schoolClassReq = this.examsSvc.getSchoolClassesByEducationLevelYear(ely);

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
        this.exams = [];
    };

   

    onButtonSearchClick = (es: ExamSearch) => {
        this.exams = [];
        if (!es.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!es.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!es.sessionId)
            this.toastr.info('Select session before searching!');
        else if (!es.educationLevelId)
            this.toastr.info('Select education level before searching!');
        else if (!es.schoolClassId)
            this.toastr.info('Select class before searching!');
        else {
            this.eduLevelId = es.educationLevelId;
            this.examsSvc.getExamsBySearch(es).subscribe({
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
}
