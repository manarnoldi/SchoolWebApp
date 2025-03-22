import {Injectable} from '@angular/core';
import {StudentSubject} from '../models/student-subject';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs/internal/Observable';
import {map} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StudentSubjectsService extends ResourceService<StudentSubject> {
    constructor(private http: HttpClient) {
        super(http, StudentSubject);
    }

    getStudentSubjectsBySchoolClassSubjectId = (
        classId: number,
        subjectId: number
    ): Observable<StudentSubject[]> => {
        return this.get(
            '/studentSubjects/bySchoolClassSubjectId/' +
                classId +
                '/' +
                subjectId
        ).pipe(
            map((examResults) =>
                examResults.sort((a, b) =>
                    a.studentClass?.student?.upi.localeCompare(
                        b.studentClass?.student?.upi
                    )
                )
            )
        );
    };
}
