import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumYear} from '@/academics/models/curriculum-year';
import {Exam} from '@/academics/models/exam';
import {ExamSearch} from '@/academics/models/exam-search';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {ExamsService} from '@/academics/services/exams.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SessionsService} from '@/class/services/sessions.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-exams',
    templateUrl: './exams.component.html',
    styleUrl: './exams.component.scss'
})
export class ExamsComponent implements OnInit {
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

    isLoading: boolean = false;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private sessionSvc: SessionsService,
        private subjectsSvc: SubjectsService,
        private schoolClassesSvc: SchoolClassesService,
        private examsSvc: ExamsService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');

        forkJoin([curriculaReq, academicYearsReq]).subscribe(
            ([curricula, academicYears]) => {
                this.curricula = curricula;
                this.academicYears = academicYears;
                this.isLoading = true;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    curriculumYearChanged = (cy: CurriculumYear) => {
        this.sessions = [];
        this.subjects = [];
        this.schoolClasses = [];
        this.exams = [];
        let sessionsReq = this.sessionSvc.get(
            `/sessions/byCurriculumYearId/${cy.curriculumId}/${cy.academicYearId}`
        );
        let subjectsReq = this.subjectsSvc.get(
            `/subjects/byCurriculumId/${cy.curriculumId}`
        );
        let schoolClassesReq = this.schoolClassesSvc.get(
            `/schoolClasses/byAcademicYearId/${cy.academicYearId}`
        );

        forkJoin([sessionsReq, subjectsReq, schoolClassesReq]).subscribe(
            ([sessions, subjects, schoolClasses]) => {
                this.sessions = sessions;
                this.subjects = subjects;
                this.schoolClasses = schoolClasses;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
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
        else {
            this.examsSvc
                .get(
                    `/exams/examSearch?academicYearId=${es.academicYearId}&curriculumId=${es.curriculumId}&sessionId=
                    ${es.sessionId}&schoolClassId=${es.schoolClassId ?? ''}&subjectId=${es.subjectId ?? ''}`
                )
                .subscribe(
                    (exams) => {
                        exams.length <= 0
                            ? this.toastr.info(
                                  'No records found with the search parameters selected!'
                              )
                            : (this.exams = exams);
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
        }
    };

    editItem(id: number) {
        // this.subjectsSvc.getById(id, '/subjects').subscribe(
        //     (res) => {
        //         let subjectId = res.id;
        //         this.subject = new SubjectGroup(res);
        //         this.subject.id = subjectId;
        //         this.subjectsAddForm.setFormControls(this.subject);
        //         this.subjectsAddForm.editMode = true;
        //         this.subjectsAddForm.subject = this.subject;
        //         this.tableButton.onClick();
        //     },
        //     (err) => {
        //         this.toastr.error(err);
        //     }
        // );
    }

    deleteItem(id: number) {
        // Swal.fire({
        //     title: `Delete record?`,
        //     text: `Confirm if you want to delete record.`,
        //     width: 400,
        //     position: 'top',
        //     padding: '1em',
        //     icon: 'question',
        //     showCancelButton: true,
        //     confirmButtonText: `Delete`,
        //     cancelButtonText: 'Cancel'
        // }).then((result) => {
        //     if (result.value) {
        //         this.subjectGroupsSvc.delete('/subjects', id).subscribe(
        //             (res) => {
        //                 this.refreshItems();
        //                 this.toastr.success('Record deleted successfully!');
        //             },
        //             (err) => {
        //                 this.toastr.error(err);
        //             }
        //         );
        //     } else if (result.dismiss === Swal.DismissReason.cancel) {
        //     }
        // });
    }
}
