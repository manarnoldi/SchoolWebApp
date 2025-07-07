import {ResourceService} from '@/core/services/resource.service';
import {Injectable} from '@angular/core';
import {Exam} from '../models/exam';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';

@Injectable({
    providedIn: 'root'
})
export class ExamsService extends ResourceService<Exam> {
    constructor(private http: HttpClient) {
        super(http, Exam);
    }

    getExamsBySearch = (ssf: SchoolSoftFilter): Observable<Exam[]> => {
        let url = `/exams/examSearch?academicYearId=${ssf.academicYearId}&curriculumId=${ssf.curriculumId}&sessionId=
              ${ssf.sessionId}&schoolClassId=${ssf.schoolClassId ?? ''}&subjectId=${ssf.subjectId ?? ''}&examTypeId=${ssf.examTypeId ?? ''}`;
        return this.get(url).pipe(map((exams) => exams));
    };
}
