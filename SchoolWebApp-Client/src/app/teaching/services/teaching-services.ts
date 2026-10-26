import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {
    CurriculumStrand,
    LessonPlan,
    LessonPlanWeek,
    LessonPlanWeekSummary,
    LessonTaughtStatus,
    RecordOfWork,
    SchemeCopySource,
    SchemeImportResult,
    SchemeOfWork,
    TeachingAssignment,
    TeachingContext,
    Tracker
} from '../models/teaching-models';

function toParams(filters: {[k: string]: any}): HttpParams {
    let params = new HttpParams();
    Object.keys(filters).forEach((k) => {
        let v = filters[k];
        if (v !== null && v !== undefined && v !== '') params = params.set(k, v);
    });
    return params;
}

@Injectable({providedIn: 'root'})
export class TeachingDocumentsService {
    constructor(private http: HttpClient) {}

    // Who the caller is and the terms. Not cached: another user can log in
    // without the page reloading.
    context(): Observable<TeachingContext> {
        return this.http.get<TeachingContext>('/teachingDocuments/context');
    }

    assignments(sessionId: number, scope: 'mine' | 'oversee'): Observable<TeachingAssignment[]> {
        return this.http.get<TeachingAssignment[]>('/teachingDocuments/assignments', {params: toParams({sessionId, scope})});
    }

    curriculum(subjectId: number, learningLevelId: number, sessionId?: number): Observable<CurriculumStrand[]> {
        return this.http.get<CurriculumStrand[]>('/teachingDocuments/curriculum', {params: toParams({subjectId, learningLevelId, sessionId})});
    }

    preferredApprovers(subjectId: number): Observable<number[]> {
        return this.http.get<number[]>('/teachingDocuments/preferredApprovers', {params: toParams({subjectId})});
    }

    tracker(filters: {sessionId: number; departmentId?: number | null; learningLevelId?: number | null;
        subjectId?: number | null; staffDetailsId?: number | null}): Observable<Tracker> {
        return this.http.get<Tracker>('/teachingDocuments/tracker', {params: toParams(filters)});
    }
}

@Injectable({providedIn: 'root'})
export class SchemesOfWorkService {
    constructor(private http: HttpClient) {}

    get(id: number): Observable<SchemeOfWork> {
        return this.http.get<SchemeOfWork>(`/schemesOfWork/${id}`);
    }

    create(subjectId: number, schoolClassId: number, sessionId: number, staffDetailsId?: number): Observable<{id: number}> {
        return this.http.post<{id: number}>('/schemesOfWork', {subjectId, schoolClassId, sessionId, staffDetailsId});
    }

    save(s: SchemeOfWork): Observable<SchemeOfWork> {
        return this.http.put<SchemeOfWork>('/schemesOfWork', {
            id: s.id, references: s.references, numberOfWeeks: s.numberOfWeeks,
            lessonsPerWeek: s.lessonsPerWeek, lessons: s.lessons
        });
    }

    generate(id: number, replace: boolean): Observable<SchemeOfWork> {
        return this.http.post<SchemeOfWork>(`/schemesOfWork/${id}/generate`, {replace});
    }

    copySources(id: number): Observable<SchemeCopySource[]> {
        return this.http.get<SchemeCopySource[]>(`/schemesOfWork/${id}/copySources`);
    }

    copyFrom(id: number, sourceId: number): Observable<SchemeOfWork> {
        return this.http.post<SchemeOfWork>(`/schemesOfWork/${id}/copyFrom/${sourceId}`, {});
    }

    delete(id: number): Observable<any> {
        return this.http.delete(`/schemesOfWork/${id}`);
    }

    // ---- Excel workbooks ----

    /** Every subject and class the teacher has that term; without staffDetailsId, the caller's own. */
    teacherWorkbook(sessionId: number, staffDetailsId?: number | null): Observable<Blob> {
        return this.http.get('/schemeImports/teacherWorkbook', {params: toParams({sessionId, staffDetailsId}), responseType: 'blob'});
    }

    schemeWorkbook(id: number): Observable<Blob> {
        return this.http.get(`/schemeImports/schemeWorkbook/${id}`, {responseType: 'blob'});
    }

    /** Checks the workbook (commit false) or saves its sheets without errors (commit true). */
    upload(file: File, commit: boolean): Observable<SchemeImportResult> {
        let form = new FormData();
        form.append('file', file, file.name);
        return this.http.post<SchemeImportResult>('/schemeImports', form, {params: toParams({commit})});
    }
}

@Injectable({providedIn: 'root'})
export class LessonPlansService {
    constructor(private http: HttpClient) {}

    weeks(schemeOfWorkId: number): Observable<LessonPlanWeekSummary[]> {
        return this.http.get<LessonPlanWeekSummary[]>('/lessonPlans/weeks', {params: toParams({schemeOfWorkId})});
    }

    createWeek(schemeOfWorkId: number, weekNo: number): Observable<{id: number}> {
        return this.http.post<{id: number}>('/lessonPlans/weeks', {schemeOfWorkId, weekNo});
    }

    week(id: number): Observable<LessonPlanWeek> {
        return this.http.get<LessonPlanWeek>(`/lessonPlans/weeks/${id}`);
    }

    deleteWeek(id: number): Observable<any> {
        return this.http.delete(`/lessonPlans/weeks/${id}`);
    }

    savePlan(p: LessonPlan): Observable<LessonPlanWeek> {
        return this.http.put<LessonPlanWeek>(`/lessonPlans/plans/${p.id}`, p);
    }

    addPlan(weekId: number): Observable<LessonPlanWeek> {
        return this.http.post<LessonPlanWeek>(`/lessonPlans/weeks/${weekId}/plans`, {});
    }

    // Brings the week up to date with its scheme; replace overwrites what the scheme supplies, else blanks only.
    refreshFromScheme(weekId: number, replace: boolean): Observable<LessonPlanWeek> {
        return this.http.post<LessonPlanWeek>(`/lessonPlans/weeks/${weekId}/refreshFromScheme?replace=${replace}`, {});
    }

    deletePlan(id: number): Observable<LessonPlanWeek> {
        return this.http.delete<LessonPlanWeek>(`/lessonPlans/plans/${id}`);
    }
}

@Injectable({providedIn: 'root'})
export class RecordOfWorkService {
    constructor(private http: HttpClient) {}

    get(schemeOfWorkId: number): Observable<RecordOfWork> {
        return this.http.get<RecordOfWork>('/recordOfWork', {params: toParams({schemeOfWorkId})});
    }

    saveEntries(schemeOfWorkId: number, entries: {schemeOfWorkLessonId: number; status: LessonTaughtStatus;
        taughtOn: string | null; workCovered: string | null; remarks: string | null}[]): Observable<RecordOfWork> {
        return this.http.put<RecordOfWork>('/recordOfWork/entries', entries, {params: toParams({schemeOfWorkId})});
    }

    check(schemeOfWorkId: number, weekNo: number, comment: string | null): Observable<RecordOfWork> {
        return this.http.post<RecordOfWork>('/recordOfWork/checks', {schemeOfWorkId, weekNo, comment});
    }

    removeCheck(id: number): Observable<RecordOfWork> {
        return this.http.delete<RecordOfWork>(`/recordOfWork/checks/${id}`);
    }
}
