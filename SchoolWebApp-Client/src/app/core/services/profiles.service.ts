import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Payslip} from '@/payroll/models/payroll-models';

export interface ProfileExamSubject {
    subject: string;
    score: number;
    outOf: number;
    percent: number;
}

export interface ProfileExam {
    schoolExamId: number;
    name: string;
    session: string;
    year: string;
    startDate: string;
    released: boolean;
    className: string;
    mean: number;
    subjects: ProfileExamSubject[];
}

export interface ProfileClassRole {
    year: string;
    className: string;
    role: string;
    description: string | null;
}

export interface ProfileElection {
    election: string;
    year: string;
    position: string;
    race: string;
    asRunningMate: boolean;
    partner: string | null;
    outcome: string;
    elected: boolean;
    votes: number | null;
}

export interface StudentActivities {
    classRoles: ProfileClassRole[];
    responsibilities: {year: string; name: string; category: string | null; description: string | null}[];
    coCurricular: {name: string; description: string | null}[];
    elections: ProfileElection[];
}

export interface StaffDuties {
    classRoles: ProfileClassRole[];
    stations: {election: string; year: string; station: string; stage: string}[];
}

export interface ProfilePayslip {
    id: number;
    payrollPeriodId: number;
    period: string;
    month: number;
    year: number;
    status: string;
    grossPay: number;
    totalDeductions: number;
    netPay: number;
}

/** The extra student and staff profile tabs (api/profiles). */
@Injectable({providedIn: 'root'})
export class ProfilesService {
    constructor(private http: HttpClient) {}

    studentAcademics(studentId: number): Observable<ProfileExam[]> {
        return this.http.get<ProfileExam[]>(`/profiles/students/${studentId}/academics`);
    }

    studentActivities(studentId: number): Observable<StudentActivities> {
        return this.http.get<StudentActivities>(`/profiles/students/${studentId}/activities`);
    }

    staffDuties(staffId: number): Observable<StaffDuties> {
        return this.http.get<StaffDuties>(`/profiles/staff/${staffId}/duties`);
    }

    staffPayslips(staffId: number): Observable<ProfilePayslip[]> {
        return this.http.get<ProfilePayslip[]>(`/profiles/staff/${staffId}/payslips`);
    }

    staffPayslip(staffId: number, payslipId: number): Observable<Payslip> {
        return this.http.get<Payslip>(`/profiles/staff/${staffId}/payslips/${payslipId}`);
    }

    studentStatement(studentId: number): Observable<any> {
        return this.http.get<any>('/financeReports/studentStatement', {params: {studentId}});
    }
}
