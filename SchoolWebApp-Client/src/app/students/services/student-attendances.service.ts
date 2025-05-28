import {Injectable} from '@angular/core';
import {StudentAttendance} from '../models/student-attendance';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StudentAttendancesService extends ResourceService<StudentAttendance> {
    constructor(private http: HttpClient) {
        super(http, StudentAttendance);
    }

    getDistinctMonths(): Observable<number[]> {
        return this.http
            .get<number[]>('/studentAttendances/getDistictMonths')
            .pipe(map((result) => result));
    }

    getDistinctYears(): Observable<number[]> {
        return this.http
            .get<number[]>('/studentAttendances/getDistictYears')
            .pipe(map((result) => result));
    }

    getByMonthYearSchoolClassId(
        month: number,
        year: number,
        schoolClassId: number
    ): Observable<StudentAttendance[]> {
        let searchStr = `/studentAttendances/byMonthYearStudentClassId/${month}/${year}/${schoolClassId}`;
        return this.get(searchStr).pipe(
            map((studentAttendance) => studentAttendance)
        );
    }
}
