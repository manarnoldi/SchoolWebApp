import {HttpClient, HttpParams} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {AuditLogPage} from '../models/audit-log';

export interface AuditLogsQuery {
    userName?: string | null;
    action?: string | null;
    entityType?: string | null;
    search?: string | null;
    startDate?: string | null;
    endDate?: string | null;
    page?: number;
    pageSize?: number;
}

@Injectable({providedIn: 'root'})
export class AuditLogsService {
    constructor(private http: HttpClient) {}

    list(q: AuditLogsQuery): Observable<AuditLogPage> {
        let params = new HttpParams();
        if (q.userName?.trim())
            params = params.set('userName', q.userName.trim());
        if (q.action?.trim())
            params = params.set('action', q.action.trim());
        if (q.entityType?.trim())
            params = params.set('entityType', q.entityType.trim());
        if (q.search?.trim()) params = params.set('search', q.search.trim());
        if (q.startDate) params = params.set('startDate', q.startDate);
        if (q.endDate) params = params.set('endDate', q.endDate);
        if (q.page) params = params.set('page', String(q.page));
        if (q.pageSize) params = params.set('pageSize', String(q.pageSize));
        return this.http.get<AuditLogPage>('/auditlogs', {params});
    }

    actions(): Observable<string[]> {
        return this.http.get<string[]>('/auditlogs/actions');
    }

    entityTypes(): Observable<string[]> {
        return this.http.get<string[]>('/auditlogs/entitytypes');
    }

    // Fire-and-forget print recording. Subscribers don't care about the
    // response — we just want the row in the table. Errors are
    // swallowed because a failed audit write must never pop a toast in
    // the middle of a user's print job.
    logPrint(
        entityType: string,
        entityId: string | number | null,
        notes?: string
    ) {
        this.http
            .post('/auditevents/print', {
                entityType,
                entityId: entityId != null ? String(entityId) : null,
                notes: notes ?? null
            })
            .subscribe({error: () => {}});
    }

    // Same shape for the logout event. Called from AuthService.doLogout
    // BEFORE the token is cleared so the request still carries the JWT.
    logLogout() {
        this.http
            .post('/auditevents/logout', {})
            .subscribe({error: () => {}});
    }
}
