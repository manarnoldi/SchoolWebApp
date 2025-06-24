import {ResourceService} from '@/core/services/resource.service';
import {Injectable} from '@angular/core';
import {StudentClass} from '../models/student-class';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {Status} from '@/core/enums/status';

@Injectable({
    providedIn: 'root'
})
export class StudentClassService extends ResourceService<StudentClass> {
    constructor(private http: HttpClient) {
        super(http, StudentClass);
    }

    getByStudentId(studentId: number): Observable<StudentClass[]> {
        let searchStr = `/studentClasses/byStudentId/${studentId}`;
        return this.get(searchStr).pipe(
            map((studentClasses) => studentClasses)
        );
    }

    getByStudentYearId(
        studentId: number,
        yearId: number
    ): Observable<StudentClass[]> {
        let searchStr = `/studentClasses/byStudentYearId/${studentId}/${yearId}`;
        return this.get(searchStr).pipe(
            map((studentClasses) => studentClasses)
        );
    }

    getBySchoolClassId = (
        schoolClassId: number,
        status: Status
    ): Observable<StudentClass[]> => {
        let searchStr = `/studentClasses/bySchoolClassId/${schoolClassId}/${status}`;
        return this.get(searchStr).pipe(
            map((studentClasses) => studentClasses)
        );
    };
}
