import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {
    BellSchedule,
    OptionGroup,
    Readiness,
    SubjectRule,
    TeacherRule,
    TimetableLookups
} from '../models/timetable-models';

/** Timetable set-up: bell schedules, subject rules, option groups, teacher limits, readiness. */
@Injectable({providedIn: 'root'})
export class TimetableSetupService {
    private base = '/timetableSetup';

    constructor(private http: HttpClient) {}

    lookups(): Observable<TimetableLookups> {
        return this.http.get<TimetableLookups>(`${this.base}/lookups`);
    }

    bellSchedules(): Observable<BellSchedule[]> {
        return this.http.get<BellSchedule[]>(`${this.base}/bellSchedules`);
    }

    saveBellSchedule(s: BellSchedule): Observable<{id: number}> {
        return s.id
            ? this.http.put<{id: number}>(`${this.base}/bellSchedules/${s.id}`, s)
            : this.http.post<{id: number}>(`${this.base}/bellSchedules`, s);
    }

    deleteBellSchedule(id: number): Observable<void> {
        return this.http.delete<void>(`${this.base}/bellSchedules/${id}`);
    }

    subjectRules(): Observable<SubjectRule[]> {
        return this.http.get<SubjectRule[]>(`${this.base}/subjectRules`);
    }

    saveSubjectRules(rules: SubjectRule[]): Observable<void> {
        return this.http.put<void>(`${this.base}/subjectRules`, rules);
    }

    optionGroups(): Observable<OptionGroup[]> {
        return this.http.get<OptionGroup[]>(`${this.base}/optionGroups`);
    }

    saveOptionGroups(groups: OptionGroup[]): Observable<void> {
        return this.http.put<void>(`${this.base}/optionGroups`, groups);
    }

    teacherRules(): Observable<TeacherRule[]> {
        return this.http.get<TeacherRule[]>(`${this.base}/teacherRules`);
    }

    saveTeacherRule(rule: TeacherRule): Observable<void> {
        return this.http.put<void>(`${this.base}/teacherRules/${rule.staffDetailsId}`, rule);
    }

    readiness(sessionId: number): Observable<Readiness> {
        return this.http.get<Readiness>(`${this.base}/readiness`, {params: new HttpParams().set('sessionId', sessionId)});
    }
}
