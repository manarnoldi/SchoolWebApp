import {Component, OnInit, ViewChild} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Budget, BudgetLine} from '@/finance/models/budget';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, BudgetService, BudgetAmendmentService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {BudgetAmendment, BudgetAmendmentStatus} from '@/finance/models/budget-master';
import {AuthService} from '@/core/services/auth.service';
import {ApprovalWebpartComponent} from '@/approvals/components/approval-webpart/approval-webpart.component';
import {ApprovalService} from '@/approvals/services/approval.service';
import {ApprovalRequestStatus} from '@/approvals/models/approval.models';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-finance-budget-amendments',
    templateUrl: './budget-amendments.component.html'
})
export class FinanceBudgetAmendmentsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/budget-amendments'], title: 'Budget Amendments'}
    ];
    dashboardTitle = 'Finance: Budget Amendments';

    amendments: BudgetAmendment[] = [];
    budgets: Budget[] = [];
    accounts: Account[] = [];
    academicYears: any[] = [];
    amendmentStatus = BudgetAmendmentStatus;

    detail: Budget | null = null;
    amendment: BudgetAmendment = new BudgetAmendment();
    showAmendmentForm: boolean = false;
    amendmentViewMode: boolean = false;

    viewDetail: any = null;
    viewBudget: Budget | null = null;

    filterYearId: any = null;
    filterBudgetId: any = null;
    filterStatus: any = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    currentUserId: number | null = null;
    approvalStatusMap: Map<number, number> = new Map();
    approvalAssigneeMap: Map<number, number | null> = new Map();
    approvalLockedMap: Map<number, boolean> = new Map();
    approvalApproverMap: Map<number, boolean> = new Map();

    @ViewChild('amendmentApprovalWebpart') amendmentApprovalWebpart?: ApprovalWebpartComponent;

    constructor(
        private toastr: ToastrService,
        private budgetSvc: BudgetService,
        private accSvc: AccountService,
        private yearSvc: AcademicYearsService,
        private amendSvc: BudgetAmendmentService,
        private authSvc: AuthService,
        private approvalSvc: ApprovalService,
        private route: ActivatedRoute
    ) {
        let cu = this.authSvc.getCurrentUser();
        this.currentUserId = cu?.id || null;
    }

    loadApprovals = () => {
        this.approvalSvc.getStatusesByEntityType('BudgetAmendment').subscribe({
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

    ngOnInit(): void {
        forkJoin([
            this.amendSvc.get('/budgetAmendments'),
            this.budgetSvc.get('/budgets'),
            this.accSvc.get('/accounts'),
            this.yearSvc.get('/academicYears')
        ]).subscribe({
            next: ([amendments, budgets, accounts, years]) => {
                this.amendments = amendments || [];
                this.budgets = budgets || [];
                this.accounts = (accounts || [])
                    .filter((a) => a.accountType === AccountType.Income || a.accountType === AccountType.Expense)
                    .sort((a, b) => (a.code || '').localeCompare(b.code || ''));
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                let active = this.academicYears.find((y: any) => y.status === true);
                this.filterYearId = active ? active.id : (this.academicYears[0]?.id || null);
                this.loadApprovals();

                let actId = this.route.snapshot.queryParamMap.get('actId');
                if (actId) {
                    let target = this.amendments.find((a: any) => +a.id === +actId);
                    if (target) this.actAmendment(target);
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load = () => {
        this.amendSvc.get('/budgetAmendments').subscribe({
            next: (a) => {
                this.amendments = a || [];
                this.loadApprovals();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    filtered = (): BudgetAmendment[] => {
        return (this.amendments || []).filter((a) => {
            let b = this.budgets.find((x) => x.id == a.budgetId);
            if (this.filterYearId != null && b && b.academicYearId != this.filterYearId) return false;
            if (this.filterBudgetId != null && a.budgetId != this.filterBudgetId) return false;
            if (this.filterStatus != null && a.status !== this.filterStatus) return false;
            return true;
        });
    };

    clearFilters = () => {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.filterYearId = active ? active.id : (this.academicYears[0]?.id || null);
        this.filterBudgetId = null;
        this.filterStatus = null;
        this.page = 1;
    };

    getBudgetName = (id: any): string => {
        let b = this.budgets.find((x) => x.id == id);
        return b?.name || '';
    };

    filteredBudgets = (): Budget[] => {
        if (!this.filterYearId) return this.budgets;
        return this.budgets.filter((b) => b.academicYearId == this.filterYearId);
    };

    getAccountsByType = (type: 'Income' | 'Expense'): Account[] => {
        let t = type === 'Income' ? AccountType.Income : AccountType.Expense;
        return this.accounts.filter((a) => a.accountType === t);
    };

    tagLineTypes = (b: Budget) => {
        (b.lines || []).forEach((l: any) => {
            if (typeof l.accountType === 'string' && (l.accountType === 'Income' || l.accountType === 'Expense')) return;
            if (typeof l.accountType === 'number') {
                l.accountType = l.accountType === AccountType.Income ? 'Income' : 'Expense';
                return;
            }
            let acc = this.accounts.find((a) => a.id == l.accountId);
            l.accountType = (acc && acc.accountType === AccountType.Income) ? 'Income' : 'Expense';
        });
    };

    addNew = () => {
        this.detail = null;
        this.amendment = new BudgetAmendment({
            budgetId: undefined,
            amendmentDate: new Date().toISOString().substring(0, 10),
            lines: []
        });
        this.amendmentViewMode = false;
        this.showAmendmentForm = true;
    };

    onAmendmentBudgetChange = () => {
        if (!this.amendment.budgetId) { this.detail = null; return; }
        this.budgetSvc.getById(+this.amendment.budgetId, '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.detail = new Budget(full);
                this.detail.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.tagLineTypes(this.detail);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    viewAmendment = (a: BudgetAmendment) => {
        if (!a.budgetId) return;
        this.budgetSvc.getById(+a.budgetId, '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.viewBudget = new Budget(full);
                this.viewBudget.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.tagLineTypes(this.viewBudget);
                this.amendSvc.getById(+a.id, '/budgetAmendments').subscribe({
                    next: (item: any) => {
                        let amend = Array.isArray(item) ? item[0] : item;
                        this.viewDetail = new BudgetAmendment(amend);
                        this.viewDetail.lines = (amend?.lines || []).map((l: any) => {
                            let acc = this.accounts.find((ac) => ac.id == l.accountId);
                            return {
                                ...l,
                                accountCode: acc?.code,
                                accountName: acc?.name,
                                accountType: acc?.accountType === AccountType.Income ? 'Income' : 'Expense',
                                delta: (+l.newAmount || 0) - (+l.previousAmount || 0)
                            };
                        });
                    },
                    error: () => this.toastr.error('Error loading amendment.')
                });
            },
            error: () => this.toastr.error('Error loading budget.')
        });
    };

    closeViewDetail = () => { this.viewDetail = null; this.viewBudget = null; };

    viewGetLinesByType = (type: 'Income' | 'Expense') => {
        if (!this.viewDetail) return [];
        return (this.viewDetail.lines || []).filter((l: any) => l.accountType === type);
    };

    viewGetTypeDelta = (type: 'Income' | 'Expense'): number => {
        return this.viewGetLinesByType(type).reduce((s: number, l: any) => s + (+l.delta || 0), 0);
    };

    // Open amendment modal in EDIT mode — submitter can change lines or pick approvers.
    editAmendment = (a: BudgetAmendment) => {
        if (!a.budgetId) return;
        this.budgetSvc.getById(+a.budgetId, '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.detail = new Budget(full);
                this.detail.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.tagLineTypes(this.detail);
                this.amendSvc.getById(+a.id, '/budgetAmendments').subscribe({
                    next: (item: any) => {
                        let amend = Array.isArray(item) ? item[0] : item;
                        this.amendment = new BudgetAmendment(amend);
                        this.amendment.amendmentDate = this.amendment.amendmentDate
                            ? (this.amendment.amendmentDate as string).substring(0, 10)
                            : '';
                        this.amendment.lines = (amend?.lines || []).map((l: any) => {
                            let acc = this.accounts.find((ac) => ac.id == l.accountId);
                            return {
                                ...l,
                                accountCode: acc?.code,
                                accountName: acc?.name,
                                accountType: acc?.accountType === AccountType.Income ? 'Income' : 'Expense',
                                delta: (+l.newAmount || 0) - (+l.previousAmount || 0)
                            };
                        });
                        this.amendmentViewMode = false;
                        this.showAmendmentForm = true;
                    },
                    error: () => this.toastr.error('Error loading amendment.')
                });
            },
            error: () => this.toastr.error('Error loading budget.')
        });
    };

    actAmendment = (a: BudgetAmendment) => {
        if (!a.budgetId) return;
        this.budgetSvc.getById(+a.budgetId, '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.detail = new Budget(full);
                this.detail.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.tagLineTypes(this.detail);
                this.amendSvc.getById(+a.id, '/budgetAmendments').subscribe({
                    next: (item: any) => {
                        let amend = Array.isArray(item) ? item[0] : item;
                        this.amendment = new BudgetAmendment(amend);
                        this.amendment.amendmentDate = this.amendment.amendmentDate
                            ? (this.amendment.amendmentDate as string).substring(0, 10)
                            : '';
                        this.amendment.lines = (amend?.lines || []).map((l: any) => {
                            let acc = this.accounts.find((ac) => ac.id == l.accountId);
                            return {
                                ...l,
                                accountCode: acc?.code,
                                accountName: acc?.name,
                                accountType: acc?.accountType === AccountType.Income ? 'Income' : 'Expense',
                                delta: (+l.newAmount || 0) - (+l.previousAmount || 0)
                            };
                        });
                        this.amendmentViewMode = true;
                        this.showAmendmentForm = true;
                    },
                    error: () => this.toastr.error('Error loading amendment.')
                });
            },
            error: () => this.toastr.error('Error loading budget.')
        });
    };

    cancelAmendment = () => {
        this.showAmendmentForm = false;
        this.amendmentViewMode = false;
        this.detail = null;
    };

    onAmendmentNewAmountChange = (line: any) => {
        line.delta = (+line.newAmount || 0) - (+line.previousAmount || 0);
    };

    addAmendmentLine = (type: 'Income' | 'Expense') => {
        this.amendment.lines.push({
            accountId: undefined,
            accountCode: '',
            accountName: '',
            previousAmount: 0,
            newAmount: 0,
            delta: 0,
            notes: '',
            accountType: type
        } as any);
    };

    removeAmendmentLine = (line: any) => {
        let i = this.amendment.lines.indexOf(line);
        if (i >= 0) this.amendment.lines.splice(i, 1);
    };

    onAmendmentAccountChange = (line: any) => {
        let acc = this.accounts.find((a) => a.id == line.accountId);
        if (acc) {
            line.accountCode = acc.code;
            line.accountName = acc.name;
        }
        let existing: any = (this.detail?.lines || []).find((l: any) => l.accountId == line.accountId);
        line.previousAmount = existing ? +((existing.effectiveAmount ?? existing.budgetedAmount) || 0) : 0;
        line.newAmount = line.previousAmount;
        this.onAmendmentNewAmountChange(line);
    };

    getAmendmentLinesByType = (type: 'Income' | 'Expense') => {
        return (this.amendment.lines || []).filter((l: any) => {
            if ((l as any).accountType) return (l as any).accountType === type;
            let acc = this.accounts.find((a) => a.id == l.accountId);
            if (!acc) return type === 'Expense';
            return (type === 'Income' && acc.accountType === AccountType.Income)
                || (type === 'Expense' && acc.accountType === AccountType.Expense);
        });
    };

    getAmendmentAvailableAccounts = (type: 'Income' | 'Expense'): Account[] => {
        let usedInAmendment = new Set((this.amendment.lines || []).map((l: any) => l.accountId).filter((x: any) => x != null));
        return this.getAccountsByType(type).filter((a) => !usedInAmendment.has(a.id));
    };

    getAmendmentTypeDelta = (type: 'Income' | 'Expense'): number => {
        return this.getAmendmentLinesByType(type).reduce((s: number, l: any) => s + (+l.delta || 0), 0);
    };

    getCurrentTypeEffective = (type: 'Income' | 'Expense'): number => {
        if (!this.detail) return 0;
        let targetEnum = type === 'Income' ? AccountType.Income : AccountType.Expense;
        return (this.detail.lines || []).filter((l: any) => {
            if (typeof l.accountType === 'string' && l.accountType.length > 0) return l.accountType === type;
            if (typeof l.accountType === 'number') return l.accountType === targetEnum;
            let acc = this.accounts.find((a) => a.id == l.accountId);
            return acc ? acc.accountType === targetEnum : type === 'Expense';
        }).reduce((s: number, l: any) => s + (+(l.effectiveAmount ?? l.budgetedAmount) || 0), 0);
    };

    saveAmendment = () => {
        if (!this.amendment.budgetId) { this.toastr.info('Select a budget.'); return; }

        // Editing an existing amendment: submission only (no update endpoint for line edits).
        if (this.amendment.id) {
            if (!this.amendmentApprovalWebpart?.hasPickerSelections()) {
                this.toastr.info('Pick approvers in the approval section to submit this amendment.');
                return;
            }
            this.amendmentApprovalWebpart.entityId = +this.amendment.id;
            this.amendmentApprovalWebpart.submitSilently().then((r) => {
                if (r) this.toastr.success('Submitted for approval.');
                this.cancelAmendment();
                this.load();
            });
            return;
        }

        // New amendment flow
        let changed = (this.amendment.lines || []).filter((l: any) => l.accountId && +l.newAmount !== +l.previousAmount);
        if (changed.length === 0) {
            this.toastr.info('No changes to amend.');
            return;
        }
        let payload = new BudgetAmendment({
            budgetId: this.amendment.budgetId,
            amendmentDate: this.amendment.amendmentDate,
            reason: this.amendment.reason,
            lines: changed.map((l: any) => ({
                accountId: l.accountId,
                previousAmount: +l.previousAmount || 0,
                newAmount: +l.newAmount || 0,
                notes: l.notes
            }) as any)
        });
        this.amendSvc.create('/budgetAmendments', payload).subscribe({
            next: (saved: any) => {
                let newId = saved?.id || null;
                if (newId && this.amendmentApprovalWebpart?.hasPickerSelections()) {
                    this.amendment.id = newId;
                    this.amendmentApprovalWebpart.entityId = newId;
                    this.amendmentApprovalWebpart.submitSilently().then((r) => {
                        this.toastr.success(r ? 'Amendment created and submitted for approval.' : 'Amendment created.');
                        this.cancelAmendment();
                        this.load();
                    });
                } else {
                    this.toastr.success('Amendment saved. You can submit for approval later via the Edit button.');
                    this.cancelAmendment();
                    this.load();
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    };

    onAmendmentApprovalChanged = (_r: any) => {
        if (_r && _r.status !== undefined) {
            this.load();
            this.cancelAmendment();
        }
    };

    deleteAmendment = (a: BudgetAmendment) => {
        Swal.fire({
            title: 'Delete amendment?', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.amendSvc.delete('/budgetAmendments', +a.id).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    getAmendmentStatusLabel = (s: any): string => {
        if (s === BudgetAmendmentStatus.Approved) return 'Approved';
        if (s === BudgetAmendmentStatus.Rejected) return 'Rejected';
        return 'Pending';
    };

    getAmendmentStatusClass = (s: any): string => {
        if (s === BudgetAmendmentStatus.Approved) return 'bg-success';
        if (s === BudgetAmendmentStatus.Rejected) return 'bg-danger';
        return 'bg-warning text-dark';
    };

    getVarianceClass = (v: number): string => {
        if (v > 0) return 'text-success';
        if (v < 0) return 'text-danger';
        return '';
    };
}
