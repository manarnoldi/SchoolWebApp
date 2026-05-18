import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Sponsor, Sponsorship, SponsorPayment} from '../models/sponsorship';
import {Account} from '../models/account';
import {Budget} from '../models/budget';
import {BudgetAmendment, BudgetMaster} from '../models/budget-master';
import {
    ExpenseCategory, Expense, FeeCategory, FeeStructure,
    JournalEntry, Payment, StudentInvoice
} from '../models/finance-models';

@Injectable({providedIn: 'root'})
export class AccountService extends ResourceService<Account> {
    constructor(private http: HttpClient) { super(http, Account); }
}

@Injectable({providedIn: 'root'})
export class FeeCategoryService extends ResourceService<FeeCategory> {
    constructor(private http: HttpClient) { super(http, FeeCategory); }
}

@Injectable({providedIn: 'root'})
export class FeeStructureService extends ResourceService<FeeStructure> {
    constructor(private http: HttpClient) { super(http, FeeStructure); }
    createFeeStructure(payload: any): Observable<any> {
        return this.http.post('/feeStructures', payload);
    }
    updateById(id: number, payload: any): Observable<any> {
        return this.http.put(`/feeStructures/${id}`, payload);
    }
}

@Injectable({providedIn: 'root'})
export class StudentInvoiceService extends ResourceService<StudentInvoice> {
    constructor(private http: HttpClient) { super(http, StudentInvoice); }
    public bulkCreate(payload: any): Observable<any> {
        return this.http.post('/studentInvoices/bulk', payload);
    }
    public updateById(id: number, payload: any): Observable<any> {
        return this.http.put(`/studentInvoices/${id}`, payload);
    }
}

@Injectable({providedIn: 'root'})
export class PaymentService extends ResourceService<Payment> {
    constructor(private http: HttpClient) { super(http, Payment); }
    creditNote(id: number, payload: any): Observable<any> {
        return this.http.post(`/payments/${id}/creditNote`, payload);
    }
    debitNote(id: number, payload: any): Observable<any> {
        return this.http.post(`/payments/${id}/debitNote`, payload);
    }
    submitNote(id: number): Observable<any> {
        return this.http.post(`/payments/${id}/submit`, {});
    }
    approveNote(id: number): Observable<any> {
        return this.http.post(`/payments/${id}/approveNote`, {});
    }
    rejectNote(id: number, reason: string): Observable<any> {
        return this.http.post(`/payments/${id}/rejectNote`, { Reason: reason });
    }
}

@Injectable({providedIn: 'root'})
export class ExpenseCategoryService extends ResourceService<ExpenseCategory> {
    constructor(private http: HttpClient) { super(http, ExpenseCategory); }
}

@Injectable({providedIn: 'root'})
export class ExpenseService extends ResourceService<Expense> {
    constructor(private http: HttpClient) { super(http, Expense); }
    updateById(id: number, payload: any): Observable<any> {
        return this.http.put(`/expenses/${id}`, payload);
    }
    submit(id: number): Observable<any> {
        return this.http.post(`/expenses/${id}/submit`, {});
    }
    approve(id: number): Observable<any> {
        return this.http.post(`/expenses/${id}/approve`, {});
    }
    reject(id: number, reason: string): Observable<any> {
        return this.http.post(`/expenses/${id}/reject`, { Reason: reason });
    }
}

@Injectable({providedIn: 'root'})
export class JournalEntryService extends ResourceService<JournalEntry> {
    constructor(private http: HttpClient) { super(http, JournalEntry); }
    submit(id: number): Observable<any> {
        return this.http.post(`/journalEntries/${id}/submit`, {});
    }
    approve(id: number): Observable<any> {
        return this.http.post(`/journalEntries/${id}/approve`, {});
    }
    reject(id: number, reason: string): Observable<any> {
        return this.http.post(`/journalEntries/${id}/reject`, { Reason: reason });
    }
}

@Injectable({providedIn: 'root'})
export class BudgetService extends ResourceService<Budget> {
    constructor(private http: HttpClient) { super(http, Budget); }
}

@Injectable({providedIn: 'root'})
export class BudgetMasterService extends ResourceService<BudgetMaster> {
    constructor(private http: HttpClient) { super(http, BudgetMaster); }
}

@Injectable({providedIn: 'root'})
export class BudgetAmendmentService extends ResourceService<BudgetAmendment> {
    constructor(private http: HttpClient) { super(http, BudgetAmendment); }
    approve(id: number, approvedById?: number): Observable<any> {
        let q = approvedById ? `?approvedById=${approvedById}` : '';
        return this.http.post(`/budgetAmendments/${id}/approve${q}`, {});
    }
    reject(id: number, approvedById?: number): Observable<any> {
        let q = approvedById ? `?approvedById=${approvedById}` : '';
        return this.http.post(`/budgetAmendments/${id}/reject${q}`, {});
    }
}

@Injectable({providedIn: 'root'})
export class FinanceReportsService {
    constructor(private http: HttpClient) {}
    trialBalance(from: string, to: string): Observable<any[]> {
        return this.http.get<any[]>(`/financeReports/trialBalance?from=${from}&to=${to}`);
    }
    incomeStatement(from: string, to: string): Observable<any> {
        return this.http.get<any>(`/financeReports/incomeStatement?from=${from}&to=${to}`);
    }
    balanceSheet(asOf: string): Observable<any> {
        return this.http.get<any>(`/financeReports/balanceSheet?asOf=${asOf}`);
    }
    feeCollection(from: string, to: string, academicYearId?: any, sessionId?: any, schoolClassId?: any): Observable<any> {
        let params: string[] = [`from=${from}`, `to=${to}`];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (schoolClassId) params.push(`schoolClassId=${schoolClassId}`);
        return this.http.get<any>(`/financeReports/feeCollection?${params.join('&')}`);
    }
    outstandingBalances(academicYearId?: any, sessionId?: any, search?: string): Observable<any[]> {
        let params: string[] = [];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (search) params.push(`search=${encodeURIComponent(search)}`);
        let q = params.length > 0 ? '?' + params.join('&') : '';
        return this.http.get<any[]>(`/financeReports/outstandingBalances${q}`);
    }
    consolidatedBudget(academicYearId: number): Observable<any> {
        return this.http.get<any>(`/financeReports/consolidatedBudget?academicYearId=${academicYearId}`);
    }
    expenseReport(from: string, to: string): Observable<any> {
        return this.http.get<any>(`/financeReports/expenseReport?from=${from}&to=${to}`);
    }
    studentStatement(studentId: number, academicYearId?: any, sessionId?: any, from?: string, to?: string): Observable<any> {
        let params: string[] = [`studentId=${studentId}`];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (from) params.push(`from=${from}`);
        if (to) params.push(`to=${to}`);
        return this.http.get<any>(`/financeReports/studentStatement?${params.join('&')}`);
    }

    feeBalancesByClass(academicYearId?: any, sessionId?: any, schoolClassId?: any): Observable<any> {
        let params: string[] = [];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (schoolClassId) params.push(`schoolClassId=${schoolClassId}`);
        let q = params.length > 0 ? '?' + params.join('&') : '';
        return this.http.get<any>(`/financeReports/feeBalancesByClass${q}`);
    }
    studentDiscounts(academicYearId?: any, sessionId?: any, schoolClassId?: any): Observable<any> {
        let params: string[] = [];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (schoolClassId) params.push(`schoolClassId=${schoolClassId}`);
        let q = params.length > 0 ? '?' + params.join('&') : '';
        return this.http.get<any>(`/financeReports/studentDiscounts${q}`);
    }
    creditDebitNotes(from: string, to: string, academicYearId?: any, sessionId?: any, schoolClassId?: any, noteType?: string, status?: string): Observable<any> {
        let params: string[] = [`from=${from}`, `to=${to}`];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (schoolClassId) params.push(`schoolClassId=${schoolClassId}`);
        if (noteType) params.push(`noteType=${noteType}`);
        if (status) params.push(`status=${status}`);
        return this.http.get<any>(`/financeReports/creditDebitNotes?${params.join('&')}`);
    }
}

@Injectable({providedIn: 'root'})
export class SponsorService {
    constructor(private http: HttpClient) {}
    getAll(): Observable<Sponsor[]> { return this.http.get<Sponsor[]>('/sponsors'); }
    getById(id: number): Observable<Sponsor> { return this.http.get<Sponsor>(`/sponsors/${id}`); }
    create(payload: any): Observable<Sponsor> { return this.http.post<Sponsor>('/sponsors', payload); }
    update(id: number, payload: any): Observable<Sponsor> { return this.http.put<Sponsor>(`/sponsors/${id}`, payload); }
    delete(id: number): Observable<any> { return this.http.delete(`/sponsors/${id}`); }
    statement(id: number, from?: string, to?: string): Observable<any> {
        let params: string[] = [];
        if (from) params.push(`from=${from}`);
        if (to) params.push(`to=${to}`);
        let q = params.length ? '?' + params.join('&') : '';
        return this.http.get<any>(`/sponsors/${id}/statement${q}`);
    }
}

@Injectable({providedIn: 'root'})
export class SponsorshipService {
    constructor(private http: HttpClient) {}
    getAll(): Observable<Sponsorship[]> { return this.http.get<Sponsorship[]>('/sponsorships'); }
    getById(id: number): Observable<Sponsorship> { return this.http.get<Sponsorship>(`/sponsorships/${id}`); }
    create(payload: any): Observable<any> { return this.http.post('/sponsorships', payload); }
    update(id: number, payload: any): Observable<any> { return this.http.put(`/sponsorships/${id}`, payload); }
    delete(id: number): Observable<any> { return this.http.delete(`/sponsorships/${id}`); }
    byStudent(studentId: number, academicYearId?: number, sessionId?: number, onDate?: string): Observable<Sponsorship[]> {
        let params: string[] = [];
        if (academicYearId) params.push(`academicYearId=${academicYearId}`);
        if (sessionId) params.push(`sessionId=${sessionId}`);
        if (onDate) params.push(`onDate=${onDate}`);
        let q = params.length ? '?' + params.join('&') : '';
        return this.http.get<Sponsorship[]>(`/sponsorships/byStudent/${studentId}${q}`);
    }
    applyToExisting(id: number): Observable<any> {
        return this.http.post(`/sponsorships/${id}/applyToExisting`, {});
    }
}

@Injectable({providedIn: 'root'})
export class SponsorPaymentService {
    constructor(private http: HttpClient) {}
    getAll(): Observable<SponsorPayment[]> { return this.http.get<SponsorPayment[]>('/sponsorPayments'); }
    create(payload: any): Observable<any> { return this.http.post('/sponsorPayments', payload); }
    delete(id: number): Observable<any> { return this.http.delete(`/sponsorPayments/${id}`); }
}
