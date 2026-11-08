import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {TimetableDetail, TimetableSummary} from '../models/timetable-models';

export interface PublishedTimetable {
    timetable: TimetableDetail | null;
    sessions: {id: number; name: string}[];
}

/** Building timetables (Timetable menus) and reading the published one (everyone). */
@Injectable({providedIn: 'root'})
export class TimetablesService {
    private base = '/timetables';

    constructor(private http: HttpClient) {}

    list(sessionId: number | null): Observable<TimetableSummary[]> {
        let params = new HttpParams();
        if (sessionId) params = params.set('sessionId', sessionId);
        return this.http.get<TimetableSummary[]>(this.base, {params});
    }

    get(id: number): Observable<TimetableDetail> {
        return this.http.get<TimetableDetail>(`${this.base}/${id}`);
    }

    create(name: string, sessionId: number): Observable<{id: number}> {
        return this.http.post<{id: number}>(this.base, {name, sessionId});
    }

    update(id: number, name: string, notes: string | null): Observable<void> {
        return this.http.put<void>(`${this.base}/${id}`, {name, notes});
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.base}/${id}`);
    }

    generate(id: number, seconds = 20): Observable<TimetableDetail> {
        return this.http.post<TimetableDetail>(`${this.base}/${id}/generate`, {seconds});
    }

    publish(id: number, effectiveFrom: string): Observable<void> {
        return this.http.post<void>(`${this.base}/${id}/publish`, {effectiveFrom});
    }

    copy(id: number): Observable<{id: number}> {
        return this.http.post<{id: number}>(`${this.base}/${id}/copy`, {});
    }

    published(sessionId?: number | null): Observable<PublishedTimetable> {
        let params = new HttpParams();
        if (sessionId) params = params.set('sessionId', sessionId);
        return this.http.get<PublishedTimetable>(`${this.base}/published`, {params});
    }

    mine(sessionId?: number | null): Observable<PublishedTimetable & {staffDetailsId: number | null}> {
        let params = new HttpParams();
        if (sessionId) params = params.set('sessionId', sessionId);
        return this.http.get<PublishedTimetable & {staffDetailsId: number | null}>(`${this.base}/mine`, {params});
    }
}
