import {Component, EventEmitter, Input, OnChanges, Output, SimpleChanges} from '@angular/core';
import {ApprovalService} from '../../services/approval.service';
import {ApprovalRequest, ApprovalRequestStatus, StepActionStatus} from '../../models/approval.models';
import {AuthService} from '@/core/services/auth.service';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-approval-history',
    templateUrl: './approval-history.component.html'
})
export class ApprovalHistoryComponent implements OnChanges {
    @Input() entityType!: string;
    @Input() entityId: number | null = null;
    @Output() reversed = new EventEmitter<ApprovalRequest>();

    request: ApprovalRequest | null = null;
    loading = false;
    isSuperAdmin: boolean = false;

    Status = ApprovalRequestStatus;
    StepStatus = StepActionStatus;

    constructor(private svc: ApprovalService, private authSvc: AuthService, private toastr: ToastrService) {
        let user = this.authSvc.getCurrentUser();
        this.isSuperAdmin = !!(user?.roles?.some((r: any) =>
            (typeof r === 'string' ? r : r?.toString() || '').toLowerCase() === 'superadministrator'));
    }

    ngOnChanges(_: SimpleChanges): void {
        if (!this.entityType || !this.entityId) {
            this.request = null;
            return;
        }
        this.loading = true;
        this.svc.getRequestForEntity(this.entityType, this.entityId).subscribe({
            next: (r) => { this.request = r; this.loading = false; },
            error: () => { this.request = null; this.loading = false; }
        });
    }

    reverse() {
        if (!this.request) return;
        Swal.fire({
            title: 'Reverse this approval?',
            html: 'This will undo the side effects (journal posting, invoice updates, etc.) and return the record to Draft.<br/><br/>' +
                  '<textarea id="swal-reverse-reason" class="swal2-textarea" placeholder="Reason for reversal (required)"></textarea>',
            width: 500, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Reverse',
            confirmButtonColor: '#d33',
            preConfirm: () => {
                let v = (document.getElementById('swal-reverse-reason') as HTMLTextAreaElement)?.value?.trim();
                if (!v) { Swal.showValidationMessage('A reversal comment is required.'); return false; }
                return v;
            }
        }).then(r => {
            if (!r.value) return;
            this.svc.reverse(this.request!.id!, r.value).subscribe({
                next: (req) => {
                    this.request = req;
                    this.toastr.success('Approval reversed. The record has been returned to draft.');
                    this.reversed.emit(req);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error reversing approval.')
            });
        });
    }

    statusLabel(s: ApprovalRequestStatus): string { return ApprovalRequestStatus[s]; }
    stepStatusLabel(s: StepActionStatus): string { return StepActionStatus[s]; }

    statusBadge(s: ApprovalRequestStatus): string {
        switch (s) {
            case ApprovalRequestStatus.Approved: return 'bg-success';
            case ApprovalRequestStatus.Rejected: return 'bg-danger';
            case ApprovalRequestStatus.Returned: return 'bg-warning text-dark';
            case ApprovalRequestStatus.Reversed: return 'bg-dark';
            case ApprovalRequestStatus.Submitted: return 'bg-warning';
            default: return 'bg-secondary';
        }
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
}
