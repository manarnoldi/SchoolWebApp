import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Budget, BudgetLine} from '@/finance/models/budget';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, BudgetService, BudgetMasterService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {DepartmentsService} from '@/school/services/departments.service';
import {BudgetMaster} from '@/finance/models/budget-master';
import {AuthService} from '@/core/services/auth.service';
import {ViewChild} from '@angular/core';
import {ApprovalWebpartComponent} from '@/approvals/components/approval-webpart/approval-webpart.component';
import {ApprovalService} from '@/approvals/services/approval.service';
import {ApprovalRequestStatus} from '@/approvals/models/approval.models';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-finance-budgets',
    templateUrl: './budgets.component.html'
})
export class FinanceBudgetsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/budgets'], title: 'Budgets'}
    ];
    dashboardTitle = 'Finance: Budgets';

    budgets: Budget[] = [];
    accounts: Account[] = [];
    academicYears: any[] = [];
    departments: any[] = [];
    budgetMasters: BudgetMaster[] = [];
    item: Budget = new Budget({isActive: true, lines: []});
    editMode: boolean = false;
    showForm: boolean = false;

    detail: Budget | null = null;

    // Filters
    filterBudgetMasterId: any = null;
    filterDepartmentId: any = null;
    filterAcademicYearId: any = null;
    filterActive: any = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    filteredBudgets = (): Budget[] => {
        return (this.budgets || []).filter((b) => {
            if (this.filterBudgetMasterId != null && b.budgetMasterId != this.filterBudgetMasterId) return false;
            if (this.filterDepartmentId != null && b.departmentId != this.filterDepartmentId) return false;
            if (this.filterAcademicYearId != null && b.academicYearId != this.filterAcademicYearId) return false;
            if (this.filterActive != null && b.isActive !== this.filterActive) return false;
            return true;
        });
    };

    search = () => { this.page = 1; };

    onFilterBudgetPlanChange = () => {
        let plan = this.budgetMasters.find((m) => m.id == this.filterBudgetMasterId);
        this.filterAcademicYearId = plan ? plan.academicYearId : null;
        this.page = 1;
    };

    clearFilters = () => {
        this.filterBudgetMasterId = null;
        this.filterDepartmentId = null;
        let activeYear = this.academicYears.find((y: any) => y.status === true);
        this.filterAcademicYearId = activeYear ? activeYear.id : (this.academicYears[0]?.id || null);
        this.filterActive = null;
        this.page = 1;
    };

    currentUserId: number | null = null;
    viewMode: boolean = false;
    approvalStatusMap: Map<number, number> = new Map();
    approvalAssigneeMap: Map<number, number | null> = new Map();
    approvalLockedMap: Map<number, boolean> = new Map();
    approvalApproverMap: Map<number, boolean> = new Map();

    @ViewChild('budgetApprovalWebpart') budgetApprovalWebpart?: ApprovalWebpartComponent;

    constructor(
        private toastr: ToastrService,
        private svc: BudgetService,
        private accSvc: AccountService,
        private yearSvc: AcademicYearsService,
        private deptSvc: DepartmentsService,
        private masterSvc: BudgetMasterService,
        private authSvc: AuthService,
        private approvalSvc: ApprovalService,
        private route: ActivatedRoute
    ) {
        let cu = this.authSvc.getCurrentUser();
        this.currentUserId = cu?.id || null;
    }

    loadApprovals = () => {
        this.approvalSvc.getStatusesByEntityType('Budget').subscribe({
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

    // True when the current user is (or was) an approver on this entity's workflow.
    // Such users cannot edit or delete the item — they can only approve / return / reject.
    isApproverForMe = (id: any): boolean => {
        if (!id) return false;
        return this.approvalApproverMap.get(+id) === true;
    };

    canEditOrDelete = (id: any): boolean => {
        return !this.isLocked(id) && !this.isApproverForMe(id);
    };

    onBudgetApprovalChanged = (_r: any) => {
        if (_r && _r.status !== undefined) {
            this.load();
            this.showForm = false;
        }
    };

    act = (b: Budget) => {
        this.svc.getById(parseInt(b.id), '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.item = new Budget(full);
                this.item.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.item.startDate = full.startDate ? full.startDate.substring(0, 10) : '';
                this.item.endDate = full.endDate ? full.endDate.substring(0, 10) : '';
                this.tagLineTypes(this.item);
                this.editMode = true;
                this.viewMode = true;
                this.showForm = true;
                this.detail = null;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/budgets'),
            this.accSvc.get('/accounts'),
            this.yearSvc.get('/academicYears'),
            this.deptSvc.get('/departments'),
            this.masterSvc.get('/budgetMasters')
        ]).subscribe({
            next: ([budgets, accounts, years, depts, masters]) => {
                this.budgets = budgets || [];
                this.accounts = (accounts || [])
                    .filter((a) => a.accountType === AccountType.Income || a.accountType === AccountType.Expense)
                    .sort((a, b) => (a.code || '').localeCompare(b.code || ''));
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                this.departments = (depts || []).sort((a: any, b: any) => (a.name || '').localeCompare(b.name || ''));
                this.budgetMasters = masters || [];
                // Default the filter to the active/current academic year
                let activeYear = this.academicYears.find((y: any) => y.status === true);
                this.filterAcademicYearId = activeYear ? activeYear.id : (this.academicYears[0]?.id || null);
                // Default the Budget Plan filter to the first Open plan matching the active year (if any)
                let defaultPlan = this.budgetMasters.find((m) => m.status === 1 && m.academicYearId == this.filterAcademicYearId)
                    || this.budgetMasters.find((m) => m.status === 1);
                if (defaultPlan) {
                    this.filterBudgetMasterId = defaultPlan.id;
                    this.filterAcademicYearId = defaultPlan.academicYearId;
                }
                this.loadApprovals();

                let actId = this.route.snapshot.queryParamMap.get('actId');
                if (actId) {
                    let target = this.budgets.find((b: any) => +b.id === +actId);
                    if (target) this.act(target);
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    getAvailableMasters = (): BudgetMaster[] => {
        // All Open plans (plan drives academic year/dates)
        return this.budgetMasters.filter((m) => m.status === 1 /* Open */);
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

    onBudgetPlanChange = () => {
        if (this.editMode) return;
        let master = this.budgetMasters.find((m) => m.id == this.item.budgetMasterId);
        if (!master) {
            this.item.academicYearId = undefined;
            this.item.startDate = '';
            this.item.endDate = '';
            return;
        }
        this.item.academicYearId = master.academicYearId;
        this.item.startDate = master.startDate ? master.startDate.substring(0, 10) : '';
        this.item.endDate = master.endDate ? master.endDate.substring(0, 10) : '';
        this.autoBudgetName();
    };

    load = () => {
        this.svc.get('/budgets').subscribe({
            next: (budgets) => {
                this.budgets = budgets || [];
                this.loadApprovals();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    addNew = () => {
        let today = new Date();
        let yearStart = new Date(today.getFullYear(), 0, 1);
        let yearEnd = new Date(today.getFullYear(), 11, 31);
        this.item = new Budget({
            isActive: true,
            startDate: yearStart.toISOString().substring(0, 10),
            endDate: yearEnd.toISOString().substring(0, 10),
            lines: []
        });
        this.editMode = false;
        this.viewMode = false;
        this.showForm = true;
    };

    autoBudgetName = () => {
        if (this.editMode) return;
        if (!this.item.departmentId || !this.item.academicYearId) return;
        let dept = this.departments.find((d: any) => d.id == this.item.departmentId);
        let year = this.academicYears.find((y: any) => y.id == this.item.academicYearId);
        if (!dept || !year) return;
        let deptPart = dept.code ? `${dept.code}-${(dept.name || '').toUpperCase()}` : (dept.name || '').toUpperCase();
        this.item.name = `${deptPart} ${year.name} BUDGET`;
    };

    getDepartmentLabel = (d: any): string => {
        return d?.code ? `${d.code} - ${d.name}` : (d?.name || '');
    };

    edit = (b: Budget) => {
        this.svc.getById(parseInt(b.id), '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.item = new Budget(full);
                this.item.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.item.startDate = full.startDate ? full.startDate.substring(0, 10) : '';
                this.item.endDate = full.endDate ? full.endDate.substring(0, 10) : '';
                this.tagLineTypes(this.item);
                this.editMode = true;
                this.viewMode = false;
                this.showForm = true;
                this.detail = null;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    cancel = () => { this.showForm = false; this.viewMode = false; };

    addLine = (type: 'Income' | 'Expense' = 'Expense') => {
        this.item.lines.push(new BudgetLine({budgetedAmount: 0, accountType: type}));
    };

    removeLine = (line: BudgetLine) => {
        let i = this.item.lines.indexOf(line);
        if (i >= 0) this.item.lines.splice(i, 1);
    };

    getAccountsByType = (type: 'Income' | 'Expense'): Account[] => {
        let t = type === 'Income' ? AccountType.Income : AccountType.Expense;
        return this.accounts.filter((a) => a.accountType === t);
    };

    getLinesByType = (type: 'Income' | 'Expense'): BudgetLine[] => {
        let targetEnum = type === 'Income' ? AccountType.Income : AccountType.Expense;
        return (this.item.lines || []).filter((l) => {
            // Prefer string tag on the line if present
            if (typeof l.accountType === 'string' && l.accountType.length > 0) {
                return l.accountType === type;
            }
            if (typeof l.accountType === 'number') {
                return l.accountType === targetEnum;
            }
            // Fallback: resolve via the accounts list
            if (l.accountId != null) {
                let acc = this.accounts.find((a) => a.id == l.accountId);
                if (acc) return acc.accountType === targetEnum;
            }
            return type === 'Expense';
        });
    };

    getTypeTotal = (type: 'Income' | 'Expense'): number => {
        return this.getLinesByType(type).reduce((s, l) => s + (+l.budgetedAmount || 0), 0);
    };

    getTotalBudgeted = (): number => {
        return (this.item.lines || []).reduce((sum, l) => sum + (+l.budgetedAmount || 0), 0);
    };

    getNetBudgeted = (): number => {
        return this.getTypeTotal('Income') - this.getTypeTotal('Expense');
    };

    save = () => {
        if (!this.item.name || !this.item.startDate || !this.item.endDate) {
            this.toastr.info('Name, start date, and end date are required.');
            return;
        }
        if (!this.item.budgetMasterId) {
            this.toastr.info('Budget Plan is required.');
            return;
        }
        if (!this.item.academicYearId || !this.item.departmentId) {
            this.toastr.info('Academic year and department are required.');
            return;
        }
        if (this.editMode && this.item.lines.length === 0) {
            this.toastr.info('Add at least one budget line.');
            return;
        }
        let req = this.editMode
            ? this.svc.update('/budgets', this.item)
            : this.svc.create('/budgets', this.item);
        req.subscribe({
            next: (saved: any) => {
                this.toastr.success('Budget saved.');
                let newId = this.editMode ? +this.item.id : (saved?.id || null);
                if (newId && this.budgetApprovalWebpart?.hasPickerSelections()) {
                    this.item.id = newId;
                    this.budgetApprovalWebpart.entityId = newId;
                    this.budgetApprovalWebpart.submitSilently().then((r) => {
                        if (r) this.toastr.success('Submitted for approval.');
                        this.showForm = false;
                        this.load();
                    });
                } else if (!this.editMode && saved?.id) {
                    this.svc.getById(+saved.id, '/budgets').subscribe({
                        next: (raw: any) => {
                            let full: any = Array.isArray(raw) ? raw[0] : raw;
                            this.item = new Budget(full);
                            this.item.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                            this.item.startDate = full.startDate ? full.startDate.substring(0, 10) : '';
                            this.item.endDate = full.endDate ? full.endDate.substring(0, 10) : '';
                            this.tagLineTypes(this.item);
                            this.editMode = true;
                            this.load();
                        }
                    });
                } else {
                    this.showForm = false;
                    this.load();
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    };

    delete = (b: Budget) => {
        Swal.fire({
            title: 'Delete budget?', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.svc.delete('/budgets', parseInt(b.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message)
                });
            }
        });
    };

    viewBudget = (b: Budget) => {
        this.svc.getById(parseInt(b.id), '/budgets').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.detail = new Budget(full);
                this.detail.lines = (full?.lines || []).map((l: any) => new BudgetLine(l));
                this.tagLineTypes(this.detail);
                this.showForm = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    closeDetail = () => { this.detail = null; };

    getDetailTotalBudgeted = (): number => {
        if (!this.detail) return 0;
        return this.detail.lines.reduce((s, l) => s + (+l.budgetedAmount || 0), 0);
    };

    getDetailTotalActual = (): number => {
        if (!this.detail) return 0;
        return this.detail.lines.reduce((s, l) => s + (+l.actualAmount || 0), 0);
    };

    getDetailTotalVariance = (): number => {
        if (!this.detail) return 0;
        return this.detail.lines.reduce((s, l) => s + (+l.variance || 0), 0);
    };

    getVarianceClass = (v: number): string => {
        if (v > 0) return 'text-success';
        if (v < 0) return 'text-danger';
        return '';
    };
}
