import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Subject} from '../models/subject';
import {map, Observable} from 'rxjs';
import {EducationLevelSubjectService} from './education-level-subject.service';

@Injectable({
    providedIn: 'root'
})
export class SubjectsService extends ResourceService<Subject> {
    constructor(
        private http: HttpClient,
        private educationLevelSubjectSvc: EducationLevelSubjectService
    ) {
        super(http, Subject);
    }

    public getByCurriculum = (curriculumId: number): Observable<Subject[]> => {
        return this.get(`/subjects/byCurriculumId/${curriculumId ?? ''}`).pipe(
            map((subjects) => subjects)
        );
    };

    public getSubjectsByEducationLevelYear = (
        educationLevelId: number,
        academicYearId: number
    ): Observable<Subject[]> => {
        return this.educationLevelSubjectSvc
            .get(
                '/educationLevelSubjects/byEducationLevelYearId/' +
                    educationLevelId +
                    '/' +
                    academicYearId
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
}
