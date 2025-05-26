import {Injectable} from '@angular/core';
import {StaffSubject} from '../models/staff-subject';
import {HttpClient} from '@angular/common/http';
import {ResourceService} from '@/core/services/resource.service';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class StaffSubjectsService extends ResourceService<StaffSubject> {
    constructor(private http: HttpClient) {
        super(http, StaffSubject);
    }

    getByStaffYearId(
        staffId: number,
        yearId: number
    ): Observable<StaffSubject[]> {
        let searchStr = `/staffSubjects/byStaffYearId/${staffId}/${yearId}`;
        return this.get(searchStr).pipe(map((staffSubjects) => staffSubjects));
    }
}
