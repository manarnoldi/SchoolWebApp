import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {Log, LogsPage} from '../models/log';

export interface LogsQuery {
    level?: string | null;
    search?: string | null;
    startDate?: string | null;
    endDate?: string | null;
    // null/undefined = open only (default), true = resolved only, false = both.
    // Mirrors the backend controller's tri-state semantics.
    resolved?: boolean | null;
    page?: number;
    pageSize?: number;
}

@Injectable({providedIn: 'root'})
export class LogsService {
    constructor(private http: HttpClient) {}

    list(q: LogsQuery): Observable<LogsPage> {
        let params = new HttpParams();
        if (q.level) params = params.set('level', q.level);
        if (q.search?.trim()) params = params.set('search', q.search.trim());
        if (q.startDate) params = params.set('startDate', q.startDate);
        if (q.endDate) params = params.set('endDate', q.endDate);
        if (q.resolved !== null && q.resolved !== undefined)
            params = params.set('resolved', String(q.resolved));
        if (q.page) params = params.set('page', String(q.page));
        if (q.pageSize) params = params.set('pageSize', String(q.pageSize));
        return this.http.get<LogsPage>('/logs', {params});
    }

    getById(id: number): Observable<Log> {
        return this.http.get<Log>(`/logs/${id}`);
    }

    levels(): Observable<string[]> {
        return this.http.get<string[]>('/logs/levels');
    }

    setResolution(id: number, resolved: boolean, note?: string): Observable<Log> {
        return this.http.post<Log>(`/logs/${id}/resolve`, {resolved, note});
    }
}
