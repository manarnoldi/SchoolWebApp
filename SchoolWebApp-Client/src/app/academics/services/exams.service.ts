import {ResourceService} from '@/core/services/resource.service';
import {Injectable} from '@angular/core';
import {Exam} from '../models/exam';
import {HttpClient} from '@angular/common/http';
import {ExamSearch} from '../models/exam-search';
import {forkJoin, map, Observable} from 'rxjs';
import {CurriculumYear} from '../models/curriculum-year';
import {Session} from '@/class/models/session';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {Subject} from '../models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {EducationLevelSubjectService} from './education-level-subject.service';
import {SessionsService} from '@/class/services/sessions.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {ToastrService} from 'ngx-toastr';
import {CurriculumService} from './curriculum.service';

@Injectable({
    providedIn: 'root'
})
export class ExamsService extends ResourceService<Exam> {
    constructor(
        private http: HttpClient,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private schoolClassesSvc: SchoolClassesService,
        private sessionSvc: SessionsService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private educationLevelSvc: EducationLevelService,
        private studentsSvc: StudentDetailsService
    ) {
        super(http, Exam);
    }

    getInitialItems = (): Observable<any[]> => {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');
        let educationLevelReq = this.educationLevelSvc.get('/educationLevels');
        let examTypesReq = this.educationLevelSvc.get('/examTypes');
        let studentsReq = this.studentsSvc.get('/students');

        return forkJoin([
            curriculaReq,
            academicYearsReq,
            educationLevelReq,
            examTypesReq,
            studentsReq
        ]).pipe(map((results) => results));
    };

    getExamsBySearch = (es: ExamSearch): Observable<Exam[]> => {
        return this.get(
            `/exams/examSearch?academicYearId=${es.academicYearId}&curriculumId=${es.curriculumId}&sessionId=
              ${es.sessionId}&schoolClassId=${es.schoolClassId ?? ''}&subjectId=${es.subjectId ?? ''}&examTypeId=${es.examTypeId ?? ''}`
        ).pipe(map((exams) => exams));
    };

    getSessionFromCurriculumYear = (
        cy: CurriculumYear
    ): Observable<Session[]> => {
        return this.sessionSvc
            .get(
                `/sessions/byCurriculumYearId?curriculumId=${cy.curriculumId}&academicYearId=${cy.academicYearId}`
            )
            .pipe(map((sessions) => sessions));
    };

    getSubjectsByEducationLevelYear = (
        ely: EducationLevelYear
    ): Observable<Subject[]> => {
        return this.educationLevelSubjectSvc
            .get(
                '/educationLevelSubjects/byEducationLevelYearId/' +
                    ely.educationLevelId +
                    '/' +
                    ely.academicYearId
            )
            .pipe(
                map((educationLevelSubjects) => {
                    let subjts = [];
                    educationLevelSubjects.forEach((els) => {
                        subjts.push(els.subject);
                    });
                    return subjts.sort((a, b) => a.rank - b.rank);
                })
            );
    };

    getSchoolClassesByEducationLevelYear = (
        ely: EducationLevelYear
    ): Observable<SchoolClass[]> => {
        return this.schoolClassesSvc
            .get(
                '/schoolClasses/byEducationLevelYearId?educationLevelId=' +
                    ely.educationLevelId +
                    '&academicYearId=' +
                    ely.academicYearId
            )
            .pipe(
                map((schoolClasses) =>
                    schoolClasses.sort((a, b) => a.rank - b.rank)
                )
            );
    };
}
