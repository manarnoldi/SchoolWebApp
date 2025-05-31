import {Injectable} from '@angular/core';
import {StaffAttendancesReport} from '../models/staff-attendances-report';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StaffAttendancesReportService extends ResourceService<StaffAttendancesReport> {
    constructor(private http: HttpClient) {
        super(http, StaffAttendancesReport);
    }

    public getStaffAttendancesReport = (
        month: number,
        year: number,
        staffCategoryId: number
    ): Observable<StaffAttendancesReport[]> => {
        let searchUrl = `/staffAttendances/getAttendanceReport/${month}/${year}/${staffCategoryId}`;
        return this.get(searchUrl).pipe(
            map((staffAttendances) => staffAttendances)
        );
    };
}
