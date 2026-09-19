import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {
    EarningType, DeductionType, TaxBand, PayrollSetting,
    EmployeeSalary, LoanAdvance, PayrollPeriod, Payslip, NssfBand, PayrollRelief
} from '../models/payroll-models';

@Injectable({providedIn: 'root'})
export class EarningTypeService extends ResourceService<EarningType> {
    constructor(private http: HttpClient) { super(http, EarningType); }
}

@Injectable({providedIn: 'root'})
export class DeductionTypeService extends ResourceService<DeductionType> {
    constructor(private http: HttpClient) { super(http, DeductionType); }
}

@Injectable({providedIn: 'root'})
export class TaxBandService extends ResourceService<TaxBand> {
    constructor(private http: HttpClient) { super(http, TaxBand); }
}

@Injectable({providedIn: 'root'})
export class NssfBandService extends ResourceService<NssfBand> {
    constructor(private http: HttpClient) { super(http, NssfBand); }
}

@Injectable({providedIn: 'root'})
export class PayrollReliefService extends ResourceService<PayrollRelief> {
    constructor(private http: HttpClient) { super(http, PayrollRelief); }
}

@Injectable({providedIn: 'root'})
export class PayrollSettingService extends ResourceService<PayrollSetting> {
    constructor(private http: HttpClient) { super(http, PayrollSetting); }
}

@Injectable({providedIn: 'root'})
export class EmployeeSalaryService extends ResourceService<EmployeeSalary> {
    constructor(private http: HttpClient) { super(http, EmployeeSalary); }
    updateById(id: number, payload: any): Observable<any> {
        return this.http.put(`/employeeSalaries/${id}`, payload);
    }
    // Upserts salary headers keyed on staffDetailsId. Line items are left alone,
    // so only the rows the user actually edited need to be sent.
    saveBatch(payload: any[]): Observable<any> {
        return this.http.post(`/employeeSalaries/batch`, payload);
    }
}

@Injectable({providedIn: 'root'})
export class LoanAdvanceService extends ResourceService<LoanAdvance> {
    constructor(private http: HttpClient) { super(http, LoanAdvance); }
}

@Injectable({providedIn: 'root'})
export class PayrollPeriodService extends ResourceService<PayrollPeriod> {
    constructor(private http: HttpClient) { super(http, PayrollPeriod); }
    process(id: number): Observable<any> {
        return this.http.post(`/payrollPeriods/${id}/process`, {});
    }
    approve(id: number): Observable<any> {
        return this.http.post(`/payrollPeriods/${id}/approve`, {});
    }
    getPayslips(id: number): Observable<Payslip[]> {
        return this.http.get<Payslip[]>(`/payrollPeriods/${id}/payslips`);
    }
    getPayslip(id: number): Observable<Payslip> {
        return this.http.get<Payslip>(`/payrollPeriods/payslip/${id}`);
    }
}
