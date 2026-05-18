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

    // Mirrors StaffSubjectsService.getByStaffYearId: returns the student's subjects
    // restricted to one academic year. The backend has no by-student+year endpoint,
    // so we fetch the student's full subject history and filter client-side via
    // studentClass.schoolClass.academicYearId.
    getByStudentYearId = (
        studentId: number,
        yearId: number
    ): Observable<StudentSubject[]> => {
        return this.get('/studentSubjects/byStudentId/' + studentId).pipe(
            map((subjects) =>
                (subjects || []).filter(
                    (s) =>
                        // academicYearId is typed as string on the model but
                        // comes back as a number-like; coerce both sides.
                        String(s.studentClass?.schoolClass?.academicYearId) ===
                        String(yearId)
                )
            )
        );
    };
}
