import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {PayrollPeriod} from '@/payroll/models/payroll-models';
import {PayrollPeriodService} from '@/payroll/services/payroll-services';
import {AuthService} from '@/core/services/auth.service';
import {ApprovalService} from '@/approvals/services/approval.service';
import {ApprovalRequestStatus} from '@/approvals/models/approval.models';

// Payroll Processing: the periods (create, process, approve), with a period's
// payslips in a pop-up to check them and fix statutory numbers and bank details.
// Payslips are printed from Payroll Reports > Payslips.
//
// A processed payroll goes through the Payroll approval workflow - reviewed, then
// approved - when the school has it on; otherwise it is approved with one click.
@Component({
    selector: 'app-payroll-periods',
    templateUrl: './payroll-periods.component.html'
})
export class PayrollPeriodsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/periods'], title: 'Payroll Processing'}
    ];
    dashboardTitle = 'Payroll: Processing';

    periods: PayrollPeriod[] = [];
    // The period whose payslips show below the table.
    selectedPeriodId: number | null = null;
    // Bumped after a (re-)process so the payslips reload.
    payslipsReload = 0;
    // The payslips pop-up for the selected period.
    payslipsOpen = false;

    // Approval: whether the workflow is on, each period's request, and the period
    // whose approval pop-up is open.
    readonly entityType = 'PayrollPeriod';
    workflowOn = false;
    currentUserId: number | null = null;
    approvals = new Map<number, {status: number; currentStepRank: number; currentAssigneeUserId: number | null}>();
    approvalFor: PayrollPeriod | null = null;
    // A link from the notifications (?actId=) opens that period's approval once the periods load.
    private openOnLoad: number | null = null;
    item: any = {month: null, year: new Date().getFullYear()};
    showForm: boolean = false;

    page: number = 1;
    pageSize: number = 15;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    months = [
        {v: 1, n: 'January'}, {v: 2, n: 'February'}, {v: 3, n: 'March'},
        {v: 4, n: 'April'}, {v: 5, n: 'May'}, {v: 6, n: 'June'},
        {v: 7, n: 'July'}, {v: 8, n: 'August'}, {v: 9, n: 'September'},
        {v: 10, n: 'October'}, {v: 11, n: 'November'}, {v: 12, n: 'December'}
    ];

    constructor(
        private toastr: ToastrService,
        private route: ActivatedRoute,
        private svc: PayrollPeriodService,
        private authSvc: AuthService,
        private approvalSvc: ApprovalService
    ) {
        this.currentUserId = this.authSvc.getCurrentUser()?.id || null;
    }

    ngOnInit(): void {
        // A link can name the period (?periodId=); otherwise the latest opens.
        let fromLink = +(this.route.snapshot.queryParamMap.get('periodId') || 0);
        if (fromLink) this.selectedPeriodId = fromLink;
        let act = +(this.route.snapshot.queryParamMap.get('actId') || 0);
        if (act) { this.selectedPeriodId = act; this.openOnLoad = act; }
        this.approvalSvc.getWorkflowByFormKey('Payroll').subscribe({
            next: (w) => this.workflowOn = !!w && !!w.isActive && (w.steps || []).length > 0,
            error: () => this.workflowOn = false
        });
        this.load();
    }

    loadApprovals = () => {
        this.approvalSvc.getStatusesByEntityType(this.entityType).subscribe({
            next: (rows) => this.approvals = new Map((rows || []).map((r) => [r.entityId, r])),
            error: () => {}
        });
    };

    approvalStatus = (p: PayrollPeriod): number | null => this.approvals.get(+p.id)?.status ?? null;

    // With the approvers: it cannot be re-processed until they return it.
    withApprovers = (p: PayrollPeriod): boolean => this.approvalStatus(p) === ApprovalRequestStatus.Submitted;

    // Waiting on the person signed in.
    awaitsMe = (p: PayrollPeriod): boolean => this.withApprovers(p)
        && this.approvals.get(+p.id)?.currentAssigneeUserId === this.currentUserId;

    openApproval = (p: PayrollPeriod) => {
        this.selectedPeriodId = +p.id;
        this.approvalFor = p;
    };

    closeApproval = () => { this.approvalFor = null; };

    // Submitted, approved, returned or rejected: refresh the list (an approval locks the period).
    onApprovalChanged = () => {
        this.load();
    };

    // afterProcess reloads the selected period's payslips as well, since a
    // (re-)process replaces them.
    load = (afterProcess = false) => {
        this.svc.get('/payrollPeriods').subscribe({
            next: (data) => {
                this.periods = data || [];
                // Keep the current choice if it still exists (it may have been
                // deleted); otherwise open the most recent period.
                if (!this.periods.some((p) => +p.id === this.selectedPeriodId))
                    this.selectedPeriodId = this.latestPeriod()?.id ? +this.latestPeriod()!.id : null;
                if (afterProcess) this.payslipsReload++;
                this.loadApprovals();
                // Keep the pop-up on the refreshed copy of its period.
                if (this.approvalFor) this.approvalFor = this.periods.find((p) => +p.id === +this.approvalFor!.id) || null;
                if (this.openOnLoad) {
                    let p = this.periods.find((x) => +x.id === this.openOnLoad);
                    this.openOnLoad = null;
                    if (p) this.openApproval(p);
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading periods.')
        });
    };

    private latestPeriod(): PayrollPeriod | undefined {
        return [...this.periods].sort((a, b) =>
            ((b.year || 0) * 100 + (b.month || 0)) - ((a.year || 0) * 100 + (a.month || 0)))[0];
    }

    // The same object each time for a given period, so the payslips panel only
    // reloads when the choice actually changes.
    selectedPeriod(): PayrollPeriod | null {
        return this.periods.find((p) => +p.id === this.selectedPeriodId) || null;
    }

    selectPeriod = (p: PayrollPeriod) => {
        this.selectedPeriodId = +p.id;
        this.payslipsOpen = true;
    };

    isSelected = (p: PayrollPeriod): boolean => +p.id === this.selectedPeriodId;

    addNew = () => {
        this.item = {month: null, year: new Date().getFullYear()};
        this.showForm = true;
    };

    cancel = () => { this.showForm = false; };

    save = () => {
        if (!this.item.month || !this.item.year) {
            this.toastr.info('Month and year are required.');
            return;
        }
        this.svc.create('/payrollPeriods', new PayrollPeriod(this.item)).subscribe({
            next: () => {
                this.toastr.success('Payroll period created.');
                this.showForm = false;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    };

    process = (p: PayrollPeriod) => {
        // A period already processed can be run again until it is approved. The
        // re-run discards the existing payslips and rebuilds them from current
        // salaries, so make that explicit rather than framing it as a first run.
        let isRerun = p.status === 1;
        Swal.fire({
            title: isRerun ? 'Re-process payroll?' : 'Process payroll?',
            text: isRerun
                ? `This discards the ${p.payslipCount || 0} existing payslip(s) for ${p.name} and rebuilds them from the current salaries. Continue?`
                : `Process payroll for ${p.name}?`,
            icon: isRerun ? 'warning' : 'question',
            showCancelButton: true,
            confirmButtonText: isRerun ? 'Re-process' : 'Process'
        }).then((r) => {
            if (r.value) {
                this.svc.process(+p.id).subscribe({
                    next: () => {
                        this.toastr.success(isRerun ? 'Payroll re-processed successfully.' : 'Payroll processed successfully.');
                        // Show the new payslips straight away for checking.
                        this.selectedPeriodId = +p.id;
                        this.payslipsOpen = true;
                        this.load(true);
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error processing.')
                });
            }
        });
    };

    approve = (p: PayrollPeriod) => {
        Swal.fire({
            title: 'Approve payroll?',
            text: `Approve payroll for ${p.name}?`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Approve'
        }).then((r) => {
            if (r.value) {
                this.svc.approve(+p.id).subscribe({
                    next: () => {
                        this.toastr.success('Payroll approved.');
                        this.load();
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error approving.')
                });
            }
        });
    };

    delete = (p: PayrollPeriod) => {
        if (p.status !== 0) return;
        Swal.fire({
            title: 'Delete period?',
            text: `Delete ${p.name}? This cannot be undone.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.svc.delete('/payrollPeriods', +p.id).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
                });
            }
        });
    };

    // The period's status, with where it is in the approval when it is processed.
    statusLabel = (p: PayrollPeriod): string => {
        if (p.status === 1) {
            let a = this.approvals.get(+p.id);
            if (a?.status === ApprovalRequestStatus.Submitted) return a.currentStepRank > 1 ? 'Awaiting final approval' : 'Awaiting review';
            if (a?.status === ApprovalRequestStatus.Returned) return 'Returned for changes';
            if (a?.status === ApprovalRequestStatus.Rejected) return 'Rejected';
        }
        return this.getStatusLabel(p.status!);
    };

    statusClass = (p: PayrollPeriod): string => {
        if (p.status === 1) {
            let s = this.approvalStatus(p);
            if (s === ApprovalRequestStatus.Submitted) return 'bg-primary';
            if (s === ApprovalRequestStatus.Returned || s === ApprovalRequestStatus.Rejected) return 'bg-danger';
        }
        return this.getStatusClass(p.status!);
    };

    getStatusLabel = (s: number): string => {
        if (s === 0) return 'Draft';
        // Processed is the pending-approval state: payslips exist but nothing is
        // committed, and the period can still be re-processed. Spell that out so
        // it does not read as a finished payroll.
        if (s === 1) return 'Processed (Not Approved)';
        if (s === 2) return 'Approved';
        if (s === 3) return 'Posted';
        return 'Unknown';
    };

    getStatusClass = (s: number): string => {
        if (s === 0) return 'bg-secondary';
        if (s === 1) return 'bg-warning text-dark';
        if (s === 2) return 'bg-success';
        if (s === 3) return 'bg-info';
        return 'bg-secondary';
    };

}
