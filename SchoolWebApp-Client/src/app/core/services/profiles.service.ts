import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Payslip, P9Employee} from '@/payroll/models/payroll-models';

export interface ProfileExamSubject {
    subject: string;
    subjectRank: number;
    score: number;
    outOf: number;
    percent: number;
    /** The grade on the education level's exam grading, e.g. ME, and its name. */
    grade: string | null;
    gradeName: string | null;
}

export interface ProfileExam {
    schoolExamId: number;
    name: string;
    sessionId: number;
    session: string;
    academicYearId: number;
    curriculumId: number;
    schoolClassId: number;
    year: string;
    sessionStart: string;
    examTypeRank: number;
    /** The class's mean in this exam, to compare against. */
    classMean: number | null;
    startDate: string;
    released: boolean;
    className: string;
    mean: number;
    meanGrade: string | null;
    meanGradeName: string | null;
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

    // The years whose P9 the staff member can take (ended, every payroll approved), newest first.
    staffP9Years(staffId: number): Observable<number[]> {
        return this.http.get<number[]>(`/profiles/staff/${staffId}/p9`);
    }

    staffP9(staffId: number, year: number): Observable<P9Employee> {
        return this.http.get<P9Employee>(`/profiles/staff/${staffId}/p9/${year}`);
    }

    studentStatement(studentId: number): Observable<any> {
        return this.http.get<any>('/financeReports/studentStatement', {params: {studentId}});
    }
}
