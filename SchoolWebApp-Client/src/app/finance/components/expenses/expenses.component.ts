import {Component, OnInit, ViewChild} from '@angular/core';
import {ApprovalWebpartComponent} from '@/approvals/components/approval-webpart/approval-webpart.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Expense, ExpenseLine, ExpenseCategory, PaymentMethod} from '@/finance/models/finance-models';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, ExpenseCategoryService, ExpenseService, BudgetService} from '@/finance/services/finance-services';
import {Budget, BudgetLine} from '@/finance/models/budget';
import {formatDate} from '@angular/common';
import {AuthService} from '@/core/services/auth.service';
import {ApprovalService} from '@/approvals/services/approval.service';
import {ApprovalRequestStatus} from '@/approvals/models/approval.models';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-finance-expenses',
    templateUrl: './expenses.component.html'
})
export class ExpensesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/expenses'], title: 'Expenses'}
    ];
    dashboardTitle = 'Finance: Expenses';

    expenses: Expense[] = [];
    categories: ExpenseCategory[] = [];
    bankAccounts: Account[] = [];
    budgets: Budget[] = [];
    selectedBudgetId: any = null;
    availableBudgetLines: BudgetLine[] = [];
    item: Expense = new Expense();
    editMode = false;
    showForm = false;
    viewDetail: Expense | null = null;

    filterDateFrom: string = '';
    filterDateTo: string = '';

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    paymentMethods = [
        {value: PaymentMethod.Cash, label: 'Cash'},
        {value: PaymentMethod.Mpesa, label: 'M-Pesa'},
        {value: PaymentMethod.BankTransfer, label: 'Bank Transfer'},
        {value: PaymentMethod.Cheque, label: 'Cheque'},
        {value: PaymentMethod.CardPayment, label: 'Card'},
        {value: PaymentMethod.Other, label: 'Other'}
    ];

    currentUserId: number | null = null;
    viewMode: boolean = false;
    approvalStatusMap: Map<number, number> = new Map();
    approvalAssigneeMap: Map<number, number | null> = new Map();
    approvalLockedMap: Map<number, boolean> = new Map();
    approvalApproverMap: Map<number, boolean> = new Map();

    @ViewChild('approvalWebpart') approvalWebpart?: ApprovalWebpartComponent;

    constructor(
        private toastr: ToastrService,
        private svc: ExpenseService,
        private catSvc: ExpenseCategoryService,
        private accSvc: AccountService,
        private budgetSvc: BudgetService,
        private authSvc: AuthService,
        private approvalSvc: ApprovalService,
        private route: ActivatedRoute
    ) {
        let cu = this.authSvc.getCurrentUser();
        this.currentUserId = cu?.id || null;
    }

    loadApprovals = () => {
        this.approvalSvc.getStatusesByEntityType('Expense').subscribe({
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

    act = (x: Expense) => {
        this.svc.getById(parseInt(x.id), '/expenses').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.item = new Expense(full);
                this.item.lines = (full?.lines || []).map((l: any) => Object.assign(new ExpenseLine(), l));
                this.item.expenseDate = full.expenseDate ? (full.expenseDate as string).substring(0, 10) : '';
                this.editMode = true;
                this.viewMode = true;
                this.showForm = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    ngOnInit(): void {
        let now = new Date();
        this.filterDateFrom = formatDate(new Date(now.getFullYear(), now.getMonth(), 1), 'yyyy-MM-dd', 'en');
        this.filterDateTo = formatDate(now, 'yyyy-MM-dd', 'en');
        forkJoin([
            this.svc.get('/expenses'),
            this.catSvc.get('/expenseCategories'),
            this.accSvc.get('/accounts'),
            this.budgetSvc.get('/budgets')
        ]).subscribe({
            next: ([expenses, categories, accounts, budgets]) => {
                this.expenses = expenses.sort((a, b) =>
                    new Date(b.expenseDate as any).getTime() - new Date(a.expenseDate as any).getTime());
                this.categories = categories.filter((c) => c.isActive);
                this.bankAccounts = accounts.filter((a) => a.accountType === AccountType.Asset);
                this.budgets = (budgets || []).filter((b) => b.isActive);
                this.loadApprovals();

                let actId = this.route.snapshot.queryParamMap.get('actId');
                if (actId) {
                    let target = this.expenses.find((e: any) => +e.id === +actId);
                    if (target) this.act(target);
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load = () => {
        this.svc.get('/expenses').subscribe({
            next: (e) => {
                this.expenses = e.sort((a, b) =>
                    new Date(b.expenseDate as any).getTime() - new Date(a.expenseDate as any).getTime());
                this.loadApprovals();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    filtered = (): Expense[] => {
        return this.expenses.filter((e) => {
            if (this.filterDateFrom && new Date(e.expenseDate as any) < new Date(this.filterDateFrom)) return false;
            if (this.filterDateTo && new Date(e.expenseDate as any) > new Date(this.filterDateTo + 'T23:59:59')) return false;
            return true;
        });
    };

    clearFilters = () => {
        let now = new Date();
        this.filterDateFrom = formatDate(new Date(now.getFullYear(), now.getMonth(), 1), 'yyyy-MM-dd', 'en');
        this.filterDateTo = formatDate(now, 'yyyy-MM-dd', 'en');
        this.page = 1;
    };

    getFilteredTotal = (): number => {
        return this.filtered().reduce((s, e) => s + (+e.totalAmount! || 0), 0);
    };

    getStatusLabel = (s: any): string => {
        let labels: any = {0: 'Draft', 1: 'Submitted', 2: 'Approved', 3: 'Rejected'};
        return labels[s] || 'Draft';
    };

    getStatusClass = (s: any): string => {
        let classes: any = {0: 'bg-secondary', 1: 'bg-warning text-dark', 2: 'bg-success', 3: 'bg-danger'};
        return classes[s] || 'bg-secondary';
    };

    addNew = () => {
        this.item = new Expense({expenseDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'), paymentMethod: PaymentMethod.Cash, status: 0});
        this.editMode = false;
        this.viewMode = false;
        this.showForm = true;
    };

    edit = (x: Expense) => {
        this.svc.getById(parseInt(x.id), '/expenses').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.item = new Expense(full);
                this.item.lines = (full?.lines || []).map((l: any) => Object.assign(new ExpenseLine(), l));
                this.item.expenseDate = full.expenseDate ? (full.expenseDate as string).substring(0, 10) : '';
                this.editMode = true;
                this.viewMode = false;
                this.showForm = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    cancel = () => { this.showForm = false; this.viewMode = false; };

    addLine = () => {
        this.item.lines.push(Object.assign(new ExpenseLine(), {amount: 0}));
    };

    removeLine = (l: ExpenseLine) => {
        let idx = this.item.lines.indexOf(l);
        if (idx >= 0) this.item.lines.splice(idx, 1);
    };

    getLineTotal = (): number => {
        return (this.item.lines || []).reduce((s, l) => s + (+l.amount! || 0), 0);
    };

    save = () => {
        if (!this.item.expenseDate) { this.toastr.warning('Date is required.'); return; }
        if (!this.item.lines || this.item.lines.length === 0) { this.toastr.info('Add at least one expense line.'); return; }
        for (let l of this.item.lines) {
            if (!l.expenseCategoryId || !l.amount) { this.toastr.info('Each line must have a category and amount.'); return; }
        }
        let payload: any = {
            ReferenceNumber: this.item.referenceNumber,
            ExpenseDate: this.item.expenseDate,
            PaymentMethod: this.item.paymentMethod,
            TransactionReference: this.item.transactionReference,
            PaidFromAccountId: this.item.paidFromAccountId,
            Status: this.item.status || 0,
            Description: this.item.description,
            Lines: (this.item.lines || []).map((l) => ({
                Id: l.id || null,
                ExpenseCategoryId: l.expenseCategoryId,
                Amount: +l.amount! || 0,
                Vendor: l.vendor,
                BudgetLineId: l.budgetLineId,
                Description: l.description
            }))
        };
        let req = this.editMode
            ? this.svc.updateById(+this.item.id, payload)
            : this.svc.create('/expenses', new Expense(payload));
        req.subscribe({
            next: (saved: any) => {
                this.toastr.success('Expense saved.');
                let newId = this.editMode ? +this.item.id : (saved?.id || null);
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

    onApprovalChanged = (_r: any) => {
        if (_r && _r.status !== undefined) {
            this.load();
            this.showForm = false;
        }
    };

    delete = (x: Expense) => {
        Swal.fire({title: 'Delete expense?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/expenses', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    viewExpense = (x: Expense) => {
        this.svc.getById(parseInt(x.id), '/expenses').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.viewDetail = new Expense(full);
                this.viewDetail.lines = (full?.lines || []).map((l: any) => Object.assign(new ExpenseLine(), l));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    closeDetail = () => { this.viewDetail = null; };

    submitExpense = (x: Expense) => {
        Swal.fire({title: 'Submit for approval?', icon: 'question', showCancelButton: true, confirmButtonText: 'Submit'}).then((r) => {
            if (r.value) {
                this.svc.submit(+x.id).subscribe({
                    next: () => { this.toastr.success('Submitted for approval.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    approveExpense = (x: Expense) => {
        Swal.fire({title: 'Approve expense?', text: 'This will post to the general ledger.', icon: 'question', showCancelButton: true, confirmButtonText: 'Approve'}).then((r) => {
            if (r.value) {
                this.svc.approve(+x.id).subscribe({
                    next: () => { this.toastr.success('Approved and posted to GL.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    rejectExpense = (x: Expense) => {
        Swal.fire({
            title: 'Reject expense?', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Reject', confirmButtonColor: '#d33',
            input: 'text', inputLabel: 'Reason for rejection', inputPlaceholder: 'Enter reason...',
            inputValidator: (value) => { if (!value) return 'Reason is required.'; return null; }
        }).then((r) => {
            if (r.value) {
                this.svc.reject(+x.id, r.value).subscribe({
                    next: () => { this.toastr.success('Expense rejected.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    getMethodLabel = (m: any): string => {
        return this.paymentMethods.find((p) => p.value === m)?.label || '';
    };
}
