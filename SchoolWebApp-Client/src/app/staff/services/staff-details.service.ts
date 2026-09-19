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

    // Updates only what payroll may change: the statutory numbers that print on a
    // payslip, plus whether the person is paid through payroll at all. The full
    // PUT replaces the whole staff record, so payroll screens must not use it.
    public updatePayrollDetails = (
        staffId: number,
        payload: {
            kraPinNo?: string | null;
            idNumber?: string | null;
            nssfNo?: string | null;
            nhifNo?: string | null;
            excludeFromPayroll: boolean;
        }
    ): Observable<any> => {
        return this.http.put(`/staffDetails/${staffId}/payrollDetails`, payload);
    };

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
