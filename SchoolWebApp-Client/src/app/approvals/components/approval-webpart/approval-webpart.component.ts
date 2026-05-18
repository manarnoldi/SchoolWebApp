import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {ApprovalService} from '../../services/approval.service';
import {ApprovalRequest, ApprovalRequestStatus, ApprovalWorkflow, StepActionStatus, UserInRole} from '../../models/approval.models';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-approval-webpart',
    templateUrl: './approval-webpart.component.html',
    styleUrl: './approval-webpart.component.scss'
})
export class ApprovalWebpartComponent implements OnChanges {
    @Input() formKey!: string;
    @Input() entityType!: string;
    @Input() entityId: number | null = null;
    @Input() currentUserId: number | null = null;
    @Output() statusChanged = new EventEmitter<ApprovalRequest | null>();
    @Output() workflowLoaded = new EventEmitter<ApprovalWorkflow | null>();
    @Output() requestLoaded = new EventEmitter<ApprovalRequest | null>();

    workflow: ApprovalWorkflow | null = null;
    request: ApprovalRequest | null = null;
    loading = false;

    stepSelections: Record<number, number> = {};
    usersByRole: Record<number, UserInRole[]> = {};

    actionComment: string = '';

    Status = ApprovalRequestStatus;
    StepStatus = StepActionStatus;

    constructor(private svc: ApprovalService, private toastr: ToastrService) {}

    ngOnChanges(_: SimpleChanges): void {
        if (!this.formKey) return;
        this.load();
    }

    load() {
        this.loading = true;
        this.workflow = null;
        this.request = null;
        this.stepSelections = {};
        this.usersByRole = {};
        this.actionComment = '';

        this.svc.getWorkflowByFormKey(this.formKey).subscribe({
            next: (w) => {
                this.workflow = w;
                this.workflowLoaded.emit(w);
                let uniqueRoles = Array.from(new Set((w.steps || []).map(s => s.roleId)));
                uniqueRoles.forEach(rid => {
                    this.svc.usersInRole(rid).subscribe({
                        next: (users) => {
                            this.usersByRole[rid] = (users || []).map(u => ({
                                ...u,
                                fullName: (u.firstName || '') + ' ' + (u.lastName || '')
                            }));
                        },
                        error: () => { this.usersByRole[rid] = []; }
                    });
                });

                if (this.entityId && this.entityId > 0) {
                    this.svc.getRequestForEntity(this.entityType, this.entityId).subscribe({
                        next: (r) => {
                            this.request = r;
                            this.loading = false;
                            this.requestLoaded.emit(r);
                            // Do not emit statusChanged here — this is a passive load, not a state transition.
                            // Parents listen to statusChanged only for genuine changes (submit/approve/reject/return).
                        },
                        error: () => { this.loading = false; }
                    });
                } else {
                    this.loading = false;
                    this.requestLoaded.emit(null);
                }
            },
            error: (err) => {
                this.loading = false;
                this.workflowLoaded.emit(null);
                if (err.status !== 404) this.toastr.error('Error loading workflow.');
            }
        });
    }

    canSubmit(): boolean {
        if (!this.workflow) return false;
        if (!this.entityId || this.entityId <= 0) return false;
        for (let s of this.workflow.steps) {
            if (!this.stepSelections[s.rank]) return false;
        }
        return true;
    }

    // Exposed for parent: submit without confirm — used when parent is doing a combined save+submit
    submitSilently(): Promise<ApprovalRequest | null> {
        return new Promise((resolve) => {
            if (!this.canSubmit()) { resolve(null); return; }
            let payload = {
                entityType: this.entityType,
                entityId: this.entityId!,
                formKey: this.formKey,
                stepAssignments: this.workflow!.steps.map(s => ({
                    stepRank: s.rank,
                    assignedToUserId: +this.stepSelections[s.rank]
                }))
            };
            this.svc.submit(payload).subscribe({
                next: (req) => { this.request = req; this.statusChanged.emit(req); resolve(req); },
                error: () => resolve(null)
            });
        });
    }

    hasPickerSelections(): boolean {
        if (!this.workflow) return false;
        return this.workflow.steps.some(s => !!this.stepSelections[s.rank]);
    }

    // Is the webpart currently showing the picker (no request OR rejected/returned/reversed)?
    isInPickerMode(): boolean {
        if (!this.workflow) return false;
        if (!this.request) return true;
        return this.request.status === ApprovalRequestStatus.Rejected
            || this.request.status === ApprovalRequestStatus.Returned
            || this.request.status === ApprovalRequestStatus.Reversed
            || this.request.status === ApprovalRequestStatus.Draft;
    }

    submit() {
        if (!this.canSubmit()) {
            this.toastr.info('Please select an approver for every step.');
            return;
        }
        let payload = {
            entityType: this.entityType,
            entityId: this.entityId!,
            formKey: this.formKey,
            stepAssignments: this.workflow!.steps.map(s => ({
                stepRank: s.rank,
                assignedToUserId: +this.stepSelections[s.rank]
            }))
        };
        Swal.fire({
            title: 'Submit for approval?',
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Submit'
        }).then(r => {
            if (!r.value) return;
            this.svc.submit(payload).subscribe({
                next: (req) => {
                    this.request = req;
                    this.toastr.success('Request submitted for approval.');
                    this.statusChanged.emit(req);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error submitting.')
            });
        });
    }

    isMyTurn(): boolean {
        if (!this.request || !this.currentUserId) return false;
        if (this.request.status !== ApprovalRequestStatus.Submitted) return false;
        let current = this.request.actions.find(a => a.stepRank === this.request!.currentStepRank);
        return current != null && current.assignedToUserId === this.currentUserId;
    }

    performAction(action: 'approve' | 'reject' | 'return') {
        if (!this.request) return;
        if ((action === 'reject' || action === 'return') && !this.actionComment?.trim()) {
            this.toastr.info(`Please add a comment/reason for ${action === 'reject' ? 'rejection' : 'return'}.`);
            return;
        }
        let title = action === 'approve' ? 'Approve this step?'
            : action === 'reject' ? 'Reject this request?'
            : 'Return this request to the submitter?';
        let btn = action === 'approve' ? 'Approve' : action === 'reject' ? 'Reject' : 'Return';
        let color = action === 'approve' ? '#28a745' : action === 'reject' ? '#d33' : '#f39c12';
        Swal.fire({
            title, width: 400, position: 'top', padding: '1em',
            icon: action === 'approve' ? 'question' : 'warning',
            showCancelButton: true, confirmButtonText: btn, confirmButtonColor: color
        }).then(r => {
            if (!r.value) return;
            this.svc.action(this.request!.id!, action, this.actionComment).subscribe({
                next: (req) => {
                    this.request = req;
                    this.actionComment = '';
                    let msg = action === 'approve' ? 'Step approved.'
                        : action === 'reject' ? 'Request rejected.'
                        : 'Request returned to submitter.';
                    this.toastr.success(msg);
                    this.statusChanged.emit(req);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error.')
            });
        });
    }

    reset() {
        this.request = null;
        this.stepSelections = {};
        this.actionComment = '';
    }

    statusLabel(s: ApprovalRequestStatus): string {
        return ApprovalRequestStatus[s];
    }
    stepStatusLabel(s: StepActionStatus): string {
        return StepActionStatus[s];
    }
    stepStatusBadge(s: StepActionStatus): string {
        switch (s) {
            case StepActionStatus.Approved: return 'bg-success';
            case StepActionStatus.Rejected: return 'bg-danger';
            case StepActionStatus.Returned: return 'bg-warning text-dark';
            case StepActionStatus.Skipped: return 'bg-secondary';
            default: return 'bg-warning';
        }
    }
    statusBadge(s: ApprovalRequestStatus): string {
        switch (s) {
            case ApprovalRequestStatus.Approved: return 'bg-success';
            case ApprovalRequestStatus.Rejected: return 'bg-danger';
            case ApprovalRequestStatus.Returned: return 'bg-warning text-dark';
            case ApprovalRequestStatus.Submitted: return 'bg-warning';
            default: return 'bg-secondary';
        }
    }

    isFinalized(): boolean {
        return this.request?.status === ApprovalRequestStatus.Approved ||
               this.request?.status === ApprovalRequestStatus.Rejected;
    }

    // Return is "not finalized" for purposes of resubmission
    isReturned(): boolean {
        return this.request?.status === ApprovalRequestStatus.Returned
            || this.request?.status === ApprovalRequestStatus.Reversed;
    }

    getLastReturnOrRejectAction(): any {
        if (!this.request) return null;
        return [...this.request.actions].reverse().find(a =>
            a.status === StepActionStatus.Rejected || a.status === StepActionStatus.Returned);
    }
}
