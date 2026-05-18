import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {FinanceReportsService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {Status} from '@/core/enums/status';
import {formatDate} from '@angular/common';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-finance-reports',
    templateUrl: './finance-reports.component.html',
    styleUrls: ['./finance-reports.component.scss']
})
export class FinanceReportsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/reports'], title: 'Finance Reports'}
    ];
    dashboardTitle = 'Finance: Reports';

    // Report group — driven by route data. One of 'fees' | 'expenses' | 'statements'.
    reportGroup: 'fees' | 'expenses' | 'statements' = 'fees';

    // Tab-to-group map; controls which tabs are visible.
    tabGroups: {[key: string]: 'fees' | 'expenses' | 'statements'} = {
        feeCollection: 'fees',
        outstanding: 'fees',
        feeBalancesByClass: 'fees',
        discounts: 'fees',
        studentStatement: 'fees',
        expenses: 'expenses',
        notes: 'expenses',
        consolidatedBudget: 'statements',
        trialBalance: 'statements',
        incomeStatement: 'statements',
        balanceSheet: 'statements'
    };

    isInGroup(tab: string): boolean {
        return this.tabGroups[tab] === this.reportGroup;
    }

    activeTab: string = 'feeCollection';

    fromDate: string = '';
    toDate: string = '';
    asOfDate: string = '';

    trialBalance: any[] = [];
    incomeStatement: any = null;
    balanceSheet: any = null;
    feeCollection: any = null;
    outstanding: any[] = [];
    allOutstanding: any[] = [];
    expenseReport: any = null;

    academicYears: any[] = [];
    selectedAcademicYearId: any = null;
    consolidatedBudget: any = null;

    // Outstanding filters
    allSessions: any[] = [];
    outstandingSessions: any[] = [];
    outstandingYearId: any = null;
    outstandingSessionId: any = null;
    outstandingSearch: string = '';

    // Fee balances by class
    feeBalanceYearId: any = null;
    feeBalanceSessionId: any = null;
    feeBalanceClassId: any = null;
    feeBalanceSessions: any[] = [];
    feeBalanceClasses: any[] = [];
    feeBalancesReport: any = null;

    // Fee collection filters
    feeCollectionYearId: any = null;
    feeCollectionSessionId: any = null;
    feeCollectionClassId: any = null;
    feeCollectionSessions: any[] = [];
    feeCollectionClasses: any[] = [];

    // Student discounts
    discountYearId: any = null;
    discountSessionId: any = null;
    discountClassId: any = null;
    discountSessions: any[] = [];
    discountClasses: any[] = [];
    discountReport: any = null;
    expandedDiscountStudents: Set<number> = new Set();

    // Credit / debit notes
    notesYearId: any = null;
    notesSessionId: any = null;
    notesClassId: any = null;
    notesType: string = '';
    notesStatus: string = '';
    notesSessions: any[] = [];
    notesClasses: any[] = [];
    notesReport: any = null;

    // Student statement
    statementYearId: any = null;
    statementSessionId: any = null;
    statementClassId: any = null;
    statementStudentId: any = null;
    statementSessions: any[] = [];
    statementClasses: any[] = [];
    statementStudents: any[] = [];
    studentStatement: any = null;
    expandedDepts: Set<number> = new Set();

    isLoading: boolean = false;
    school: any = null;

    constructor(
        private toastr: ToastrService,
        private reportsSvc: FinanceReportsService,
        private yearSvc: AcademicYearsService,
        private schoolSvc: SchoolDetailsService,
        private sessionSvc: SessionsService,
        private classSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        // Pick up the group from the route — 'fees' | 'expenses' | 'statements'.
        let group = this.route.snapshot.data?.['reportGroup'] || 'fees';
        this.reportGroup = group;
        let defaults: any = {fees: 'feeCollection', expenses: 'expenses', statements: 'trialBalance'};
        this.activeTab = defaults[this.reportGroup] || 'feeCollection';
        let titles: any = {fees: 'Fee Reports', expenses: 'Expenses Report', statements: 'Statements'};
        this.dashboardTitle = 'Finance: ' + (titles[this.reportGroup] || 'Reports');
        this.breadcrumbs = [
            {link: ['/'], title: 'Dashboard'},
            {link: ['/finance/reports/' + this.reportGroup], title: titles[this.reportGroup]}
        ];

        let today = new Date();
        let yearStart = new Date(today.getFullYear(), 0, 1);
        this.fromDate = formatDate(yearStart, 'yyyy-MM-dd', 'en');
        this.toDate = formatDate(today, 'yyyy-MM-dd', 'en');
        this.asOfDate = formatDate(today, 'yyyy-MM-dd', 'en');

        this.schoolSvc.get('/schoolDetails').subscribe({
            next: (schools: any[]) => { this.school = schools?.[0] || null; },
            error: () => {}
        });
        this.yearSvc.get('/academicYears').subscribe({
            next: (years) => {
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                let active = this.academicYears.find((y: any) => y.status === true);
                this.selectedAcademicYearId = active ? active.id : (this.academicYears[0]?.id || null);
                this.outstandingYearId = this.selectedAcademicYearId;
                this.onOutstandingYearChange();
                this.feeBalanceYearId = this.selectedAcademicYearId;
                this.onFeeBalanceYearChange();
                this.feeCollectionYearId = this.selectedAcademicYearId;
                this.onFeeCollectionYearChange();
                this.statementYearId = this.selectedAcademicYearId;
                this.onStatementYearChange();
                this.discountYearId = this.selectedAcademicYearId;
                this.onDiscountYearChange();
                this.notesYearId = this.selectedAcademicYearId;
                this.onNotesYearChange();
            },
            error: () => {}
        });
        this.sessionSvc.get('/sessions').subscribe({
            next: (sess: any[]) => {
                this.allSessions = sess || [];
                this.onOutstandingYearChange();
                this.onFeeBalanceYearChange();
                this.onFeeCollectionYearChange();
                this.onStatementYearChange();
                this.onDiscountYearChange();
                this.onNotesYearChange();
            },
            error: () => {}
        });
    }

    switchTab(tab: string) {
        this.activeTab = tab;
    }

    getAccountTypeLabel(t: any): string {
        const labels: any = {1: 'Asset', 2: 'Liability', 3: 'Equity', 4: 'Income', 5: 'Expense'};
        return labels[t] || '';
    }

    loadTrialBalance() {
        if (!this.fromDate || !this.toDate) return;
        this.isLoading = true;
        this.reportsSvc.trialBalance(this.fromDate, this.toDate).subscribe({
            next: (r) => { this.trialBalance = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    loadIncomeStatement() {
        if (!this.fromDate || !this.toDate) return;
        this.isLoading = true;
        this.reportsSvc.incomeStatement(this.fromDate, this.toDate).subscribe({
            next: (r) => { this.incomeStatement = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    loadBalanceSheet() {
        if (!this.asOfDate) return;
        this.isLoading = true;
        this.reportsSvc.balanceSheet(this.asOfDate).subscribe({
            next: (r) => { this.balanceSheet = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    loadFeeCollection() {
        this.isLoading = true;
        this.reportsSvc.feeCollection(this.fromDate, this.toDate, this.feeCollectionYearId, this.feeCollectionSessionId, this.feeCollectionClassId).subscribe({
            next: (r) => { this.feeCollection = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    onFeeCollectionYearChange() {
        this.feeCollectionSessions = this.feeCollectionYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.feeCollectionYearId)
            : [];
        this.feeCollectionSessionId = null;
        this.feeCollectionClasses = [];
        this.feeCollectionClassId = null;
        if (this.feeCollectionYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.feeCollectionYearId}`).subscribe({
                next: (classes: any) => {
                    this.feeCollectionClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                },
                error: () => {}
            });
        }
    }

    clearFeeCollectionFilters() {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.feeCollectionYearId = active ? active.id : null;
        this.onFeeCollectionYearChange();
    }

    // ========== Student Statement ==========
    onStatementYearChange() {
        this.statementSessions = this.statementYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.statementYearId)
            : [];
        this.statementSessionId = null;
        this.statementClasses = [];
        this.statementClassId = null;
        this.statementStudents = [];
        this.statementStudentId = null;
        this.studentStatement = null;
        if (this.statementYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.statementYearId}`).subscribe({
                next: (classes: any) => {
                    this.statementClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                },
                error: () => {}
            });
        }
    }

    onStatementClassChange() {
        this.statementStudents = [];
        this.statementStudentId = null;
        this.studentStatement = null;
        if (!this.statementClassId) return;
        this.studentClassSvc.getBySchoolClassId(+this.statementClassId, Status.Active).subscribe({
            next: (scs: any[]) => {
                this.statementStudents = (scs || [])
                    .filter((sc: any) => sc.student)
                    .map((sc: any) => {
                        let s = sc.student;
                        s.displayName = `${s.upi || ''} - ${s.fullName || ''}`;
                        return s;
                    })
                    .sort((a: any, b: any) => (a.fullName || '').localeCompare(b.fullName || ''));
            },
            error: () => {}
        });
    }

    loadStudentStatement() {
        if (!this.statementStudentId) {
            this.toastr.info('Please select a student.');
            return;
        }
        this.isLoading = true;
        this.reportsSvc.studentStatement(+this.statementStudentId, this.statementYearId, this.statementSessionId).subscribe({
            next: (r) => { this.studentStatement = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    clearStatementFilters() {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.statementYearId = active ? active.id : null;
        this.onStatementYearChange();
        this.studentStatement = null;
    }

    getStatementYearName(): string {
        if (!this.studentStatement?.academicYearId) return 'All Years';
        let y = this.academicYears.find((yr: any) => yr.id == this.studentStatement.academicYearId);
        return y?.name || '';
    }

    getStatementSessionName(): string {
        if (!this.studentStatement?.sessionId) return 'All Terms';
        let s = this.allSessions.find((ss: any) => ss.id == this.studentStatement.sessionId);
        return s?.sessionName || '';
    }

    getStatementStudentClass(): string {
        let cls = this.statementClasses.find((c: any) => c.id == this.statementClassId);
        if (!cls) return '';
        return `${cls.learningLevel?.name || ''}${cls.schoolStream?.name ? ' - ' + cls.schoolStream.name : ''}`;
    }

    onOutstandingYearChange() {
        this.outstandingSessions = this.outstandingYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.outstandingYearId)
            : [];
        let activeSession = this.outstandingSessions.find((s: any) => s.status === true);
        this.outstandingSessionId = activeSession ? activeSession.id : null;
    }

    loadOutstanding() {
        this.isLoading = true;
        // Load without the search — we'll apply search client-side so it's instant as the user types
        this.reportsSvc.outstandingBalances(this.outstandingYearId, this.outstandingSessionId).subscribe({
            next: (r) => {
                this.allOutstanding = r || [];
                this.applyOutstandingSearch();
                this.isLoading = false;
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    applyOutstandingSearch() {
        if (!this.outstandingSearch) {
            this.outstanding = this.allOutstanding;
            return;
        }
        let q = this.outstandingSearch.trim().toLowerCase();
        this.outstanding = this.allOutstanding.filter((r: any) =>
            (r.studentName || '').toLowerCase().includes(q) ||
            (r.studentUPI || '').toLowerCase().includes(q)
        );
    }

    clearOutstandingFilters() {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.outstandingYearId = active ? active.id : null;
        this.onOutstandingYearChange();
        this.outstandingSearch = '';
        this.applyOutstandingSearch();
    }

    // ========== Fee Balances by Class ==========
    onFeeBalanceYearChange() {
        this.feeBalanceSessions = this.feeBalanceYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.feeBalanceYearId)
            : [];
        this.feeBalanceSessionId = null;
        this.feeBalanceClasses = [];
        this.feeBalanceClassId = null;
        if (this.feeBalanceYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.feeBalanceYearId}`).subscribe({
                next: (classes: any) => {
                    this.feeBalanceClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                },
                error: () => {}
            });
        }
    }

    loadFeeBalancesByClass() {
        this.isLoading = true;
        this.reportsSvc.feeBalancesByClass(this.feeBalanceYearId, this.feeBalanceSessionId, this.feeBalanceClassId).subscribe({
            next: (r) => { this.feeBalancesReport = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    clearFeeBalanceFilters() {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.feeBalanceYearId = active ? active.id : null;
        this.onFeeBalanceYearChange();
    }

    getClassBalance(cls: any): number {
        return (cls.students || []).reduce((s: number, r: any) => s + (+r.balance || 0), 0);
    }

    loadExpenseReport() {
        if (!this.fromDate || !this.toDate) return;
        this.isLoading = true;
        this.reportsSvc.expenseReport(this.fromDate, this.toDate).subscribe({
            next: (r) => { this.expenseReport = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    // ========== Student Discounts ==========
    onDiscountYearChange() {
        this.discountSessions = this.discountYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.discountYearId)
            : [];
        let activeSession = this.discountSessions.find((s: any) => s.status === true);
        this.discountSessionId = activeSession ? activeSession.id : null;
        this.discountClasses = [];
        this.discountClassId = null;
        if (this.discountYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.discountYearId}`).subscribe({
                next: (classes: any) => {
                    this.discountClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                },
                error: () => {}
            });
        }
    }

    loadStudentDiscounts() {
        this.isLoading = true;
        this.reportsSvc.studentDiscounts(this.discountYearId, this.discountSessionId, this.discountClassId).subscribe({
            next: (r) => {
                this.discountReport = r;
                this.expandedDiscountStudents.clear();
                this.isLoading = false;
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    clearDiscountFilters() {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.discountYearId = active ? active.id : null;
        this.onDiscountYearChange();
        this.discountReport = null;
    }

    toggleDiscountStudent(studentId: number) {
        if (this.expandedDiscountStudents.has(studentId)) this.expandedDiscountStudents.delete(studentId);
        else this.expandedDiscountStudents.add(studentId);
    }

    isDiscountStudentExpanded(studentId: number): boolean {
        return this.expandedDiscountStudents.has(studentId);
    }

    // ========== Credit / Debit Notes ==========
    onNotesYearChange() {
        this.notesSessions = this.notesYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.notesYearId)
            : [];
        let activeSession = this.notesSessions.find((s: any) => s.status === true);
        this.notesSessionId = activeSession ? activeSession.id : null;
        this.notesClasses = [];
        this.notesClassId = null;
        if (this.notesYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.notesYearId}`).subscribe({
                next: (classes: any) => {
                    this.notesClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                },
                error: () => {}
            });
        }
    }

    loadCreditDebitNotes() {
        if (!this.fromDate || !this.toDate) return;
        this.isLoading = true;
        this.reportsSvc.creditDebitNotes(this.fromDate, this.toDate,
            this.notesYearId, this.notesSessionId, this.notesClassId,
            this.notesType, this.notesStatus).subscribe({
            next: (r) => { this.notesReport = r; this.isLoading = false; },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    clearNotesFilters() {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.notesYearId = active ? active.id : null;
        this.onNotesYearChange();
        this.notesType = '';
        this.notesStatus = '';
        this.notesReport = null;
    }

    getStatusBadge(status: string): string {
        const map: any = {Draft: 'bg-secondary', Submitted: 'bg-warning', Approved: 'bg-success', Rejected: 'bg-danger'};
        return map[status] || 'bg-secondary';
    }

    printReport() { window.print(); }

    tbTotalDebit(): number { return (this.trialBalance || []).reduce((s, r) => s + (r.debit || 0), 0); }
    tbTotalCredit(): number { return (this.trialBalance || []).reduce((s, r) => s + (r.credit || 0), 0); }
    outstandingTotal(): number { return (this.outstanding || []).reduce((s, r) => s + (r.balance || 0), 0); }

    loadConsolidatedBudget() {
        if (!this.selectedAcademicYearId) { this.toastr.info('Select an academic year.'); return; }
        this.isLoading = true;
        this.reportsSvc.consolidatedBudget(+this.selectedAcademicYearId).subscribe({
            next: (r) => {
                this.consolidatedBudget = r;
                this.expandedDepts.clear();
                this.isLoading = false;
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error?.message || 'Error.'); }
        });
    }

    toggleDept(deptId: number) {
        if (this.expandedDepts.has(deptId)) this.expandedDepts.delete(deptId);
        else this.expandedDepts.add(deptId);
    }

    isDeptExpanded(deptId: number): boolean {
        return this.expandedDepts.has(deptId);
    }

    getVarianceClass(v: number): string {
        if (v > 0) return 'text-success';
        if (v < 0) return 'text-danger';
        return '';
    }
}
