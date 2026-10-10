import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {
    CommunicationSetting,
    CommunicationStatus,
    ComposeMessage,
    ComposePreview,
    MessageBatch,
    MessageQueuePage,
    MessageTemplate,
    Placeholder,
    QueueResult,
    RecipientOptions,
    SmsReport
} from '../models/communication-models';

function toParams(filters: {[k: string]: any}): HttpParams {
    let params = new HttpParams();
    Object.keys(filters).forEach((k) => {
        let v = filters[k];
        if (v !== null && v !== undefined && v !== '') params = params.set(k, v);
    });
    return params;
}

@Injectable({providedIn: 'root'})
export class CommunicationSettingsService {
    constructor(private http: HttpClient) {}

    status(): Observable<CommunicationStatus> {
        return this.http.get<CommunicationStatus>('/communicationSettings/status');
    }

    get(): Observable<CommunicationSetting> {
        return this.http.get<CommunicationSetting>('/communicationSettings');
    }

    update(s: CommunicationSetting): Observable<any> {
        return this.http.put('/communicationSettings', s);
    }

    testSms(destination: string, message?: string): Observable<any> {
        return this.http.post('/communicationSettings/testSms', {destination, message});
    }

    testEmail(destination: string, message?: string): Observable<any> {
        return this.http.post('/communicationSettings/testEmail', {destination, message});
    }

    smsBalance(): Observable<{raw: string}> {
        return this.http.get<{raw: string}>('/communicationSettings/smsBalance');
    }
}

@Injectable({providedIn: 'root'})
export class MessageTemplateService {
    constructor(private http: HttpClient) {}

    getAll(): Observable<MessageTemplate[]> {
        return this.http.get<MessageTemplate[]>('/messageTemplates');
    }

    placeholders(code?: string): Observable<Placeholder[]> {
        return this.http.get<Placeholder[]>('/messageTemplates/placeholders', {params: toParams({code})});
    }

    create(t: MessageTemplate): Observable<MessageTemplate> {
        return this.http.post<MessageTemplate>('/messageTemplates', t);
    }

    update(t: MessageTemplate): Observable<MessageTemplate> {
        return this.http.put<MessageTemplate>('/messageTemplates', t);
    }

    delete(id: number): Observable<any> {
        return this.http.delete(`/messageTemplates/${id}`);
    }
}

@Injectable({providedIn: 'root'})
export class MessageService {
    constructor(private http: HttpClient) {}

    recipientOptions(): Observable<RecipientOptions> {
        return this.http.get<RecipientOptions>('/messages/recipientOptions');
    }

    preview(m: ComposeMessage): Observable<ComposePreview> {
        return this.http.post<ComposePreview>('/messages/preview', m);
    }

    send(m: ComposeMessage): Observable<QueueResult> {
        return this.http.post<QueueResult>('/messages/send', m);
    }

    queue(filters: {[k: string]: any}): Observable<MessageQueuePage> {
        return this.http.get<MessageQueuePage>('/messages/queue', {params: toParams(filters)});
    }

    batches(filters: {[k: string]: any}): Observable<{data: MessageBatch[]; totalCount: number}> {
        return this.http.get<{data: MessageBatch[]; totalCount: number}>('/messages/batches', {params: toParams(filters)});
    }

    retry(id: number): Observable<any> {
        return this.http.post(`/messages/${id}/retry`, {});
    }

    cancel(id: number): Observable<any> {
        return this.http.post(`/messages/${id}/cancel`, {});
    }

    retryBatch(id: number): Observable<any> {
        return this.http.post(`/messages/batches/${id}/retryFailed`, {});
    }

    cancelBatch(id: number): Observable<any> {
        return this.http.post(`/messages/batches/${id}/cancel`, {});
    }

    smsReport(year: number, month: number, includeDetails: boolean): Observable<SmsReport> {
        return this.http.get<SmsReport>('/messages/smsReport', {params: toParams({year, month, includeDetails})});
    }
}
