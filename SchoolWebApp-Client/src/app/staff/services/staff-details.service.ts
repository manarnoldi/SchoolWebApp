import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {StaffDetails} from '../models/staff-details';
import {map, Observable} from 'rxjs';
import {Status} from '@/core/enums/status';

@Injectable({
    providedIn: 'root'
})
export class StaffDetailsService extends ResourceService<StaffDetails> {
    constructor(private http: HttpClient) {
        super(http, StaffDetails);
    }

    public getBySearchDetails = (
        status: Status,
        employmenttypeId: number,
        staffCategoryId: number
    ): Observable<StaffDetails[]> => {
        let searchString = `/staffDetails/staffSearch?employmentTypeId=${
            employmenttypeId ?? ''
        }&status=${status ?? ''}&staffCategoryId=${staffCategoryId ?? ''}`;
        return this.get(searchString).pipe(map((staffs) => staffs));
    };
}
