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

    searchStudentAttendancesObservable = (
        studentClassId: number,
        currentRptMonth: number
    ): Observable<StudentAttendance[]> => {
        return this.getByMonthSchoolClassId(
            currentRptMonth,
            studentClassId
        );
    };

    // getByMonthYearStudentId(
    //     month: number,
    //     year: number,
    //     staffId: number
    // ): Observable<StaffAttendance[]> {
    //     let searchStr = `/staffAttendances/byMonthYearStaffId/${month}/${year}/${staffId}`;
    //     return this.get(searchStr).pipe(
    //         map((staffAttendances) => staffAttendances)
    //     );
    // }

    getByMonthSchoolClassId(
        month: number,
        studentClassId: number
    ): Observable<StudentAttendance[]> {
        let searchStr = `/studentAttendances/byMonthStudentClassId/${month}/${studentClassId}`;
        return this.get(searchStr).pipe(
            map((studentAttendance) => studentAttendance)
        );
    }
}
