import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, Subject, tap} from 'rxjs';
import {ApprovalRequest, ApprovalWorkflow, UserInRole} from '../models/approval.models';

@Injectable({providedIn: 'root'})
export class ApprovalService {
    private refreshSubject = new Subject<void>();
    // Consumers (e.g. the notifications bell) subscribe to get pushed a signal
    // after any submit/action/reverse, so they can refresh immediately.
    public refresh$ = this.refreshSubject.asObservable();

    constructor(private http: HttpClient) {}

    notifyRefresh() { this.refreshSubject.next(); }

    getAllWorkflows(): Observable<ApprovalWorkflow[]> {
        return this.http.get<ApprovalWorkflow[]>('/approvalWorkflows');
    }
    getWorkflowById(id: number): Observable<ApprovalWorkflow> {
        return this.http.get<ApprovalWorkflow>(`/approvalWorkflows/${id}`);
    }
    getWorkflowByFormKey(formKey: string): Observable<ApprovalWorkflow> {
        return this.http.get<ApprovalWorkflow>(`/approvalWorkflows/byFormKey/${formKey}`);
    }
    createWorkflow(payload: any): Observable<ApprovalWorkflow> {
        return this.http.post<ApprovalWorkflow>('/approvalWorkflows', payload);
    }
    updateWorkflow(id: number, payload: any): Observable<ApprovalWorkflow> {
        return this.http.put<ApprovalWorkflow>(`/approvalWorkflows/${id}`, payload);
    }
    deleteWorkflow(id: number): Observable<any> {
        return this.http.delete(`/approvalWorkflows/${id}`);
    }
    usersInRole(roleId: number): Observable<UserInRole[]> {
        return this.http.get<UserInRole[]>(`/approvalWorkflows/usersInRole/${roleId}`);
    }

    getRequestForEntity(entityType: string, entityId: number): Observable<ApprovalRequest | null> {
        return this.http.get<ApprovalRequest>(`/approvalRequests/for?entityType=${encodeURIComponent(entityType)}&entityId=${entityId}`);
    }
    getStatusesByEntityType(entityType: string): Observable<{entityId: number; status: number; currentStepRank: number; currentAssigneeUserId: number | null; submittedById: number | null; isApproverForMe: boolean; isLocked: boolean}[]> {
        return this.http.get<any[]>(`/approvalRequests/statuses?entityType=${encodeURIComponent(entityType)}`);
    }
    getMyPending(): Observable<any[]> {
        return this.http.get<any[]>('/approvalRequests/myPending');
    }
    submit(payload: {entityType: string; entityId: number; formKey: string; stepAssignments: {stepRank: number; assignedToUserId: number}[]}): Observable<ApprovalRequest> {
        return this.http.post<ApprovalRequest>('/approvalRequests/submit', payload)
            .pipe(tap(() => this.notifyRefresh()));
    }
    action(requestId: number, action: 'approve' | 'reject' | 'return', comment?: string): Observable<ApprovalRequest> {
        return this.http.post<ApprovalRequest>(`/approvalRequests/${requestId}/action`, {action, comment})
            .pipe(tap(() => this.notifyRefresh()));
    }
    reverse(requestId: number, comment: string): Observable<ApprovalRequest> {
        return this.http.post<ApprovalRequest>(`/approvalRequests/${requestId}/reverse`, {comment})
            .pipe(tap(() => this.notifyRefresh()));
    }
}
