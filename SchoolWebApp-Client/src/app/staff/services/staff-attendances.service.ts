import {ResourceService} from '@/core/services/resource.service';
import {Injectable} from '@angular/core';
import {StaffAttendance} from '../models/staff-attendance';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StaffAttendancesService extends ResourceService<StaffAttendance> {
    constructor(private http: HttpClient) {
        super(http, StaffAttendance);
    }

    getDistinctMonths(): Observable<number[]> {
        return this.http
            .get<number[]>('/staffAttendances/getDistictMonths')
            .pipe(map((result) => result));
    }

    getDistinctYears(): Observable<number[]> {
        return this.http
            .get<number[]>('/staffAttendances/getDistictYears')
            .pipe(map((result) => result));
    }

    getByMonthYearStaffId(
        month: number,
        year: number,
        staffId: number
    ): Observable<StaffAttendance[]> {
        let searchStr = `/staffAttendances/byMonthYearStaffId/${month}/${year}/${staffId}`;
        return this.get(searchStr).pipe(
            map((staffAttendances) => staffAttendances)
        );
    }
}
