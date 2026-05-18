import {Component, OnInit, ViewChild} from '@angular/core';
import {ApprovalWebpartComponent} from '@/approvals/components/approval-webpart/approval-webpart.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {JournalEntry, JournalLine} from '@/finance/models/finance-models';
import {Account} from '@/finance/models/account';
import {AccountService, JournalEntryService} from '@/finance/services/finance-services';
import {formatDate} from '@angular/common';
import {AuthService} from '@/core/services/auth.service';
import {ApprovalService} from '@/approvals/services/approval.service';
import {ApprovalRequestStatus} from '@/approvals/models/approval.models';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-journal-entries',
    templateUrl: './journal-entries.component.html'
})
export class JournalEntriesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/journal-entries'], title: 'Journal Entries'}
    ];
    dashboardTitle = 'Finance: Journal Entries';

    entries: JournalEntry[] = [];
    accounts: Account[] = [];
    entry: JournalEntry = new JournalEntry();
    showForm = false;
    viewDetail: JournalEntry | null = null;

    filterDateFrom: string = '';
    filterDateTo: string = '';

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    currentUserId: number | null = null;
    viewMode: boolean = false;
    editMode: boolean = false;
    approvalStatusMap: Map<number, number> = new Map();
    approvalAssigneeMap: Map<number, number | null> = new Map();
    approvalLockedMap: Map<number, boolean> = new Map();
    approvalApproverMap: Map<number, boolean> = new Map();

    @ViewChild('approvalWebpart') approvalWebpart?: ApprovalWebpartComponent;

    constructor(private toastr: ToastrService, private svc: JournalEntryService, private accSvc: AccountService, private authSvc: AuthService, private approvalSvc: ApprovalService, private route: ActivatedRoute) {
        let cu = this.authSvc.getCurrentUser();
        this.currentUserId = cu?.id || null;
    }

    loadApprovals = () => {
        this.approvalSvc.getStatusesByEntityType('JournalEntry').subscribe({
            next: (rows) => {
                this.approvalStatusMap = new Map((rows || []).map(r => [r.entityId, r.status]));
                this.approvalAssigneeMap = new Map((rows || []).map(r => [r.entityId, r.currentAssigneeUserId]));
                this.approvalLockedMap = new Map((rows || []).map(r => [r.entityId, !!r.isLocked]));
                this.approvalApproverMap = new Map((rows || []).map(r => [r.entityId, !!r.isApproverForMe]));
            },
            error: () => {}
        });
    };

    isFullyApproved = (id: any): boolean => {
        if (!id) return false;
        return this.approvalStatusMap.get(+id) === ApprovalRequestStatus.Approved;
    };

    isMyTurn = (id: any): boolean => {
        if (!id || !this.currentUserId) return false;
        let s = this.approvalStatusMap.get(+id);
        if (s !== ApprovalRequestStatus.Submitted) return false;
        return this.approvalAssigneeMap.get(+id) === this.currentUserId;
    };

    isLocked = (id: any): boolean => {
        if (!id) return false;
        return this.approvalLockedMap.get(+id) === true;
    };

    isApproverForMe = (id: any): boolean => {
        if (!id) return false;
        return this.approvalApproverMap.get(+id) === true;
    };

    canEditOrDelete = (id: any): boolean => {
        return !this.isLocked(id) && !this.isApproverForMe(id);
    };

    act = (e: JournalEntry) => {
        this.svc.getById(parseInt(e.id), '/journalEntries').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.entry = new JournalEntry(full);
                this.entry.lines = full?.lines || [];
                this.entry.entryDate = full.entryDate ? (full.entryDate as string).substring(0, 10) : '';
                this.viewMode = true;
                this.editMode = true;
                this.showForm = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onApprovalChanged = (_r: any) => {
        if (_r && _r.status !== undefined) {
            this.load();
            this.showForm = false;
        }
    };

    ngOnInit(): void {
        let now = new Date();
        this.filterDateFrom = formatDate(new Date(now.getFullYear(), now.getMonth(), 1), 'yyyy-MM-dd', 'en');
        this.filterDateTo = formatDate(now, 'yyyy-MM-dd', 'en');
        forkJoin([this.svc.get('/journalEntries'), this.accSvc.get('/accounts')]).subscribe({
            next: ([entries, accounts]) => {
                this.entries = entries.sort((a, b) => new Date(b.entryDate as any).getTime() - new Date(a.entryDate as any).getTime());
                this.accounts = accounts.sort((a, b) => (a.code || '').localeCompare(b.code || ''));
                this.loadApprovals();

                let actId = this.route.snapshot.queryParamMap.get('actId');
                if (actId) {
                    let target = this.entries.find((e: any) => +e.id === +actId);
                    if (target) this.act(target);
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load = () => {
        this.svc.get('/journalEntries').subscribe({
            next: (e) => {
                this.entries = e.sort((a, b) => new Date(b.entryDate as any).getTime() - new Date(a.entryDate as any).getTime());
                this.loadApprovals();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    filtered = (): JournalEntry[] => {
        return this.entries.filter((e) => {
            if (this.filterDateFrom && new Date(e.entryDate as any) < new Date(this.filterDateFrom)) return false;
            if (this.filterDateTo && new Date(e.entryDate as any) > new Date(this.filterDateTo + 'T23:59:59')) return false;
            return true;
        });
    };

    clearFilters = () => {
        let now = new Date();
        this.filterDateFrom = formatDate(new Date(now.getFullYear(), now.getMonth(), 1), 'yyyy-MM-dd', 'en');
        this.filterDateTo = formatDate(now, 'yyyy-MM-dd', 'en');
        this.page = 1;
    };

    getFilteredTotalDebit = (): number => {
        return this.filtered().reduce((s, e) => s + (+e.totalDebit! || 0), 0);
    };

    addNew = () => {
        this.entry = new JournalEntry({
            entryDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
            isPosted: true,
            lines: [
                {accountId: null, debit: 0, credit: 0, description: ''},
                {accountId: null, debit: 0, credit: 0, description: ''}
            ]
        });
        this.viewMode = false;
        this.editMode = false;
        this.showForm = true;
    };

    cancel = () => { this.showForm = false; this.viewMode = false; this.editMode = false; };

    addLine = () => {
        this.entry.lines?.push({accountId: null, debit: 0, credit: 0, description: ''});
    };

    removeLine = (idx: number) => {
        this.entry.lines?.splice(idx, 1);
    };

    totalDebit = (): number => {
        return (this.entry.lines || []).reduce((s, l) => s + (+l.debit || 0), 0);
    };

    totalCredit = (): number => {
        return (this.entry.lines || []).reduce((s, l) => s + (+l.credit || 0), 0);
    };

    isBalanced = (): boolean => {
        return Math.round(this.totalDebit() * 100) === Math.round(this.totalCredit() * 100) && this.totalDebit() > 0;
    };

    save = () => {
        if (!this.isBalanced()) { this.toastr.warning('Debits must equal credits and total must be > 0.'); return; }
        let validLines = (this.entry.lines || []).filter((l) => l.accountId && ((l.debit || 0) > 0 || (l.credit || 0) > 0));
        if (validLines.length < 2) { this.toastr.warning('At least 2 valid lines required.'); return; }
        this.entry.lines = validLines;
        this.svc.create('/journalEntries', this.entry).subscribe({
            next: (saved: any) => {
                this.toastr.success('Journal entry saved.');
                let newId = saved?.id || null;
                if (newId && this.approvalWebpart?.hasPickerSelections()) {
                    this.approvalWebpart.entityId = newId;
                    this.approvalWebpart.submitSilently().then((r) => {
                        if (r) this.toastr.success('Submitted for approval.');
                        this.showForm = false;
                        this.load();
                    });
                } else {
                    this.showForm = false;
                    this.load();
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    };

    viewEntry = (e: JournalEntry) => {
        this.svc.getById(parseInt(e.id), '/journalEntries').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.viewDetail = new JournalEntry(full);
                this.viewDetail.lines = full?.lines || [];
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    closeDetail = () => { this.viewDetail = null; };

    getAccountName = (accountId: any): string => {
        let a = this.accounts.find((acc) => acc.id == accountId);
        return a ? `${a.code} - ${a.name}` : '';
    };

    getNormalBalance = (accountId: any): string => {
        let a = this.accounts.find((acc: any) => acc.id == accountId);
        if (!a) return '';
        // Asset=1, Expense=5 → normally Debit; Liability=2, Equity=3, Income=4 → normally Credit
        if (a.accountType === 1 || a.accountType === 5) return 'Normally Debit';
        if (a.accountType === 2 || a.accountType === 3 || a.accountType === 4) return 'Normally Credit';
        return '';
    };

    getNormalBalanceClass = (accountId: any): string => {
        let a = this.accounts.find((acc: any) => acc.id == accountId);
        if (!a) return '';
        if (a.accountType === 1 || a.accountType === 5) return 'text-primary';
        return 'text-success';
    };

    getStatusLabel = (s: any): string => {
        let labels: any = {0: 'Draft', 1: 'Submitted', 2: 'Approved', 3: 'Rejected'};
        return labels[s] || 'Draft';
    };

    getStatusClass = (s: any): string => {
        let classes: any = {0: 'bg-secondary', 1: 'bg-warning text-dark', 2: 'bg-success', 3: 'bg-danger'};
        return classes[s] || 'bg-secondary';
    };

    submitEntry = (e: JournalEntry) => {
        Swal.fire({title: 'Submit for approval?', icon: 'question', showCancelButton: true, confirmButtonText: 'Submit'}).then((r) => {
            if (r.value) {
                this.svc.submit(+e.id).subscribe({
                    next: () => { this.toastr.success('Submitted for approval.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    approveEntry = (e: JournalEntry) => {
        Swal.fire({title: 'Approve and post?', text: 'This will post the entry to the ledger.', icon: 'question', showCancelButton: true, confirmButtonText: 'Approve'}).then((r) => {
            if (r.value) {
                this.svc.approve(+e.id).subscribe({
                    next: () => { this.toastr.success('Approved and posted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    rejectEntry = (e: JournalEntry) => {
        Swal.fire({
            title: 'Reject entry?', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Reject', confirmButtonColor: '#d33',
            input: 'text', inputLabel: 'Reason for rejection', inputPlaceholder: 'Enter reason...',
            inputValidator: (value) => { if (!value) return 'Reason is required.'; return null; }
        }).then((r) => {
            if (r.value) {
                this.svc.reject(+e.id, r.value).subscribe({
                    next: () => { this.toastr.success('Entry rejected.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    delete = (e: JournalEntry) => {
        Swal.fire({title: 'Delete entry?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/journalEntries', parseInt(e.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };
}
