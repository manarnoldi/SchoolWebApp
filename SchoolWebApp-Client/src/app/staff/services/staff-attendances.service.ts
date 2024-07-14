import {ResourceService} from '@/core/services/resource.service';
import {Injectable} from '@angular/core';
import {StaffAttendance} from '../models/staff-attendance';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class StaffAttendancesService extends ResourceService<StaffAttendance> {
    constructor(private http: HttpClient) {
        super(http, StaffAttendance);
    }
}
