import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Payment, PaymentMethod, PaymentType, StudentInvoice, StudentInvoiceItem} from '@/finance/models/finance-models';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, PaymentService, StudentInvoiceService} from '@/finance/services/finance-services';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {Status} from '@/core/enums/status';
import {formatDate} from '@angular/common';
import {AuthService} from '@/core/services/auth.service';
import {ViewChild} from '@angular/core';
import {ApprovalWebpartComponent} from '@/approvals/components/approval-webpart/approval-webpart.component';
import {ApprovalService} from '@/approvals/services/approval.service';
import {ApprovalRequestStatus} from '@/approvals/models/approval.models';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-finance-payments',
    templateUrl: './payments.component.html'
})
export class PaymentsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/payments'], title: 'Fee Payments'}
    ];
    dashboardTitle = 'Finance: Fee Payments';

    payments: Payment[] = [];
    academicYears: any[] = [];
    allSessions: any[] = [];
    formSessions: any[] = [];
    schoolClasses: any[] = [];
    students: any[] = [];
    studentInvoices: StudentInvoice[] = [];
    bankAccounts: Account[] = [];
    item: Payment = new Payment();
    showForm = false;
    apportionMode: string = 'auto';
    selectedInvoice: StudentInvoice | null = null;
    itemAllocations: {invoiceItemId: number, feeCategoryName: string, amount: number, balance: number, allocation: number}[] = [];

    formYearId: any = null;
    formSessionId: any = null;
    formClassId: any = null;

    filterStudentSearch: string = '';
    filterMethodId: any = null;
    filterDateFrom: string = '';
    filterDateTo: string = '';
    filterYearId: any = null;
    filterSessionId: any = null;
    filterClassId: any = null;
    filterSessions: any[] = [];
    filterClasses: any[] = [];
    filterClassStudentIds: Set<number> = new Set();
    studentClassMap: Map<number, string> = new Map();
    invoiceMap: Map<number, any> = new Map();

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
    noteViewMode: boolean = false;
    noteId: number | null = null;
    approvalStatusMap: Map<number, number> = new Map();
    approvalAssigneeMap: Map<number, number | null> = new Map();
    approvalLockedMap: Map<number, boolean> = new Map();
    approvalApproverMap: Map<number, boolean> = new Map();

    @ViewChild('noteApprovalWebpart') noteApprovalWebpart?: ApprovalWebpartComponent;

    constructor(
        private toastr: ToastrService,
        private svc: PaymentService,
        private studentsSvc: StudentDetailsService,
        private invoiceSvc: StudentInvoiceService,
        private accSvc: AccountService,
        private settingSvc: GlobalSettingService,
        private yearSvc: AcademicYearsService,
        private sessionSvc: SessionsService,
        private classSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private authSvc: AuthService,
        private approvalSvc: ApprovalService,
        private route: ActivatedRoute
    ) {
        let cu = this.authSvc.getCurrentUser();
        this.currentUserId = cu?.id || null;
    }

    act = (p: Payment) => {
        // Open the credit/debit note modal in view/approve mode
        this.noteType = p.paymentType == 1 ? 'credit' : 'debit';
        this.notePayment = p;
        this.noteAmount = +p.amount!;
        this.noteReason = p.reason || '';
        this.noteViewMode = true;
        this.noteId = +p.id;
        this.showNoteForm = true;
    };

    onNoteApprovalChanged = (_r: any) => {
        if (_r && _r.status !== undefined) {
            this.load();
            this.showNoteForm = false;
        }
    };

    loadApprovals = () => {
        this.approvalSvc.getStatusesByEntityType('CreditDebitNote').subscribe({
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

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/payments'),
            this.accSvc.get('/accounts'),
            this.yearSvc.get('/academicYears'),
            this.sessionSvc.get('/sessions'),
            this.invoiceSvc.get('/studentInvoices')
        ]).subscribe({
            next: ([payments, accounts, years, sessions, invoices]: any) => {
                this.payments = payments.sort((a: any, b: any) => new Date(b.paymentDate).getTime() - new Date(a.paymentDate).getTime());
                this.bankAccounts = accounts.filter((a: any) => a.accountType === AccountType.Asset);
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                this.allSessions = sessions || [];
                // Build invoice lookup for year/session display per payment
                this.invoiceMap = new Map();
                (invoices || []).forEach((inv: any) => this.invoiceMap.set(+inv.id, inv));
                // Default to active year + active session
                let activeYear = this.academicYears.find((y: any) => y.status === true);
                if (activeYear) {
                    this.filterYearId = activeYear.id;
                    this.onFilterYearChange();
                    let activeSession = this.filterSessions.find((s: any) => s.status === true);
                    if (activeSession) this.filterSessionId = activeSession.id;
                }
                this.loadApprovals();

                let actId = this.route.snapshot.queryParamMap.get('actId');
                if (actId) {
                    let target = this.payments.find((p: any) => +p.id === +actId);
                    if (target) this.act(target);
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
        this.settingSvc.get('/globalSettings').subscribe({
            next: (settings: any[]) => {
                let mode = settings.find((s: any) => s.module === 'Finance' && s.settingKey === 'PaymentApportionMode');
                this.apportionMode = mode?.settingValue || 'auto';
            },
            error: () => {}
        });
    }

    load = () => {
        this.svc.get('/payments').subscribe({
            next: (p) => {
                this.payments = p.sort((a, b) => new Date(b.paymentDate as any).getTime() - new Date(a.paymentDate as any).getTime());
                this.loadApprovals();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onFilterYearChange = () => {
        this.filterSessions = this.filterYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.filterYearId)
            : [];
        this.filterSessionId = null;
        this.filterClassId = null;
        this.filterClasses = [];
        this.filterClassStudentIds = new Set();
        this.studentClassMap = new Map();
        if (this.filterYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.filterYearId}`).subscribe({
                next: (classes: any[]) => {
                    this.filterClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                    // Build student→class display map
                    this.filterClasses.forEach((c: any) => {
                        this.studentClassSvc.getBySchoolClassId(+c.id, Status.Active).subscribe({
                            next: (scs: any[]) => {
                                let label = `${c.learningLevel?.name || ''}${c.schoolStream?.name ? ' - ' + c.schoolStream.name : ''}`;
                                (scs || []).forEach((sc: any) => this.studentClassMap.set(sc.studentId, label));
                            },
                            error: () => {}
                        });
                    });
                },
                error: () => {}
            });
        }
    };

    getStudentClass = (studentId: any): string => {
        if (!studentId) return '';
        return this.studentClassMap.get(+studentId) || '';
    };

    getPaymentYear = (p: any): string => {
        if (!p.studentInvoiceId) return '';
        let inv = this.invoiceMap.get(+p.studentInvoiceId);
        return inv?.academicYearName || '';
    };

    getPaymentSession = (p: any): string => {
        if (!p.studentInvoiceId) return '';
        let inv = this.invoiceMap.get(+p.studentInvoiceId);
        return inv?.sessionName || '';
    };

    onFilterClassChange = () => {
        this.filterClassStudentIds = new Set();
        if (!this.filterClassId) return;
        this.studentClassSvc.getBySchoolClassId(+this.filterClassId, Status.Active).subscribe({
            next: (scs: any[]) => {
                this.filterClassStudentIds = new Set((scs || []).map((sc: any) => sc.studentId));
            },
            error: () => {}
        });
    };

    filtered = (): Payment[] => {
        return this.payments.filter((p) => {
            let inv = p.studentInvoiceId ? this.invoiceMap.get(+p.studentInvoiceId) : null;
            if (this.filterYearId != null && inv && inv.academicYearId != this.filterYearId) return false;
            if (this.filterSessionId != null && inv && inv.sessionId != this.filterSessionId) return false;
            if (this.filterClassId != null && !this.filterClassStudentIds.has(+p.studentId!)) return false;
            if (this.filterStudentSearch) {
                let q = this.filterStudentSearch.toLowerCase();
                let name = (p.studentName || '').toLowerCase();
                let upi = (p.studentUPI || '').toLowerCase();
                if (!name.includes(q) && !upi.includes(q)) return false;
            }
            if (this.filterMethodId != null && p.paymentMethod != this.filterMethodId) return false;
            if (this.filterDateFrom && new Date(p.paymentDate as any) < new Date(this.filterDateFrom)) return false;
            if (this.filterDateTo && new Date(p.paymentDate as any) > new Date(this.filterDateTo + 'T23:59:59')) return false;
            return true;
        });
    };

    clearFilters = () => {
        this.filterStudentSearch = '';
        this.filterMethodId = null;
        this.filterDateFrom = '';
        this.filterDateTo = '';
        this.filterClassId = null;
        this.filterClasses = [];
        this.filterClassStudentIds = new Set();
        let activeYear = this.academicYears.find((y: any) => y.status === true);
        this.filterYearId = activeYear ? activeYear.id : null;
        this.onFilterYearChange();
        let activeSession = this.filterSessions.find((s: any) => s.status === true);
        this.filterSessionId = activeSession ? activeSession.id : null;
        this.page = 1;
    };

    getFilteredTotal = (): number => {
        return this.filtered().reduce((s, p) => s + (+p.amount! || 0), 0);
    };

    addNew = () => {
        this.item = new Payment({paymentDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'), paymentMethod: PaymentMethod.Cash});
        this.formYearId = null;
        this.formSessionId = null;
        this.formClassId = null;
        this.formSessions = [];
        this.schoolClasses = [];
        this.students = [];
        this.studentInvoices = [];
        this.selectedInvoice = null;
        this.itemAllocations = [];
        this.showForm = true;
    };

    onFormYearChange = () => {
        this.formSessionId = null;
        this.formClassId = null;
        this.schoolClasses = [];
        this.students = [];
        this.item.studentId = undefined;
        this.studentInvoices = [];
        this.formSessions = this.formYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.formYearId)
            : [];
        if (this.formYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.formYearId}`).subscribe({
                next: (classes: any) => this.schoolClasses = classes,
                error: () => {}
            });
        }
    };

    onFormClassChange = () => {
        this.students = [];
        this.item.studentId = undefined;
        this.studentInvoices = [];
        if (!this.formClassId) return;
        this.studentClassSvc.getBySchoolClassId(+this.formClassId, Status.Active).subscribe({
            next: (scs: any[]) => {
                this.students = scs
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
    };

    onStudentChange = () => {
        this.item.studentInvoiceId = null;
        this.studentInvoices = [];
        this.selectedInvoice = null;
        this.itemAllocations = [];
        if (!this.item.studentId) return;
        // Filter invoices by session if selected
        this.invoiceSvc.get('/studentInvoices/byStudentId/' + this.item.studentId).subscribe({
            next: (invs) => {
                this.studentInvoices = invs.filter((i) => {
                    if ((i.balance || 0) <= 0) return false;
                    if (this.formSessionId && i.sessionId != this.formSessionId) return false;
                    return true;
                });
            },
            error: () => {}
        });
    };

    onInvoiceChange = () => {
        this.selectedInvoice = null;
        this.itemAllocations = [];
        if (!this.item.studentInvoiceId) return;
        this.invoiceSvc.getById(+this.item.studentInvoiceId, '/studentInvoices').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.selectedInvoice = new StudentInvoice(full);
                this.selectedInvoice.items = (full?.items || []).map((i: any) => Object.assign(new StudentInvoiceItem(), i));
                this.buildAllocations();
            },
            error: () => {}
        });
    };

    onAmountChange = () => {
        if (this.selectedInvoice && this.apportionMode === 'auto') {
            this.computeAutoAllocations();
        }
    };

    buildAllocations = () => {
        if (!this.selectedInvoice) return;
        // Build allocation rows from invoice items with remaining balance, sorted by fee category rank
        let items = (this.selectedInvoice.items || [])
            .map((it: any) => ({
                invoiceItemId: it.id,
                feeCategoryId: it.feeCategoryId,
                feeCategoryName: it.feeCategoryName || '',
                amount: +it.amount - (+it.discount || 0),
                balance: +it.amount - (+it.discount || 0) - (+it.paidAmount || 0),
                allocation: 0
            }))
            .filter((a: any) => a.balance > 0);

        // Sort by fee category rank (we use the categories list if loaded, else keep order)
        this.itemAllocations = items;
        if (this.apportionMode === 'auto') {
            this.computeAutoAllocations();
        }
    };

    computeAutoAllocations = () => {
        let remaining = +this.item.amount! || 0;
        for (let a of this.itemAllocations) {
            if (remaining <= 0) { a.allocation = 0; continue; }
            let apply = Math.min(remaining, a.balance);
            a.allocation = apply;
            remaining -= apply;
        }
    };

    getAllocationTotal = (): number => {
        return this.itemAllocations.reduce((s, a) => s + (+a.allocation || 0), 0);
    };

    cancel = () => { this.showForm = false; };

    save = () => {
        if (!this.item.studentId || !this.item.amount || !this.item.paymentDate) {
            this.toastr.warning('Student, date and amount are required.'); return;
        }
        if (this.apportionMode === 'manual' && this.item.studentInvoiceId && this.itemAllocations.length > 0) {
            let allocTotal = this.getAllocationTotal();
            if (allocTotal > 0 && Math.abs(allocTotal - (+this.item.amount! || 0)) > 0.01) {
                this.toastr.warning(`Allocation total (${allocTotal.toFixed(2)}) must equal payment amount (${(+this.item.amount!).toFixed(2)}).`);
                return;
            }
        }
        let payload: any = this.item.toJson();
        if (this.apportionMode === 'manual' && this.itemAllocations.length > 0) {
            payload.ItemAllocations = this.itemAllocations
                .filter((a) => +a.allocation > 0)
                .map((a) => ({InvoiceItemId: a.invoiceItemId, Amount: +a.allocation}));
        }
        this.svc.create('/payments', new Payment(payload)).subscribe({
            next: () => { this.toastr.success('Payment recorded.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    };

    // --- View Detail ---
    viewDetail: Payment | null = null;

    viewPayment = (p: Payment) => {
        this.svc.getById(parseInt(p.id), '/payments').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.viewDetail = new Payment(full);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    closeDetail = () => { this.viewDetail = null; };

    getAllocTotal = (): number => {
        if (!this.viewDetail?.allocations) return 0;
        return this.viewDetail.allocations.reduce((s, a) => s + (+a.amount! || 0), 0);
    };

    // --- Credit / Debit Notes ---
    showNoteForm = false;
    noteType: 'credit' | 'debit' = 'credit';
    notePayment: Payment | null = null;
    noteAmount: number = 0;
    noteReason: string = '';

    openCreditNote = (p: Payment) => {
        this.noteType = 'credit';
        this.notePayment = p;
        this.noteAmount = +p.amount!;
        this.noteReason = '';
        this.noteViewMode = false;
        this.noteId = null;
        this.showNoteForm = true;
    };

    openDebitNote = (p: Payment) => {
        this.noteType = 'debit';
        this.notePayment = p;
        this.noteAmount = 0;
        this.noteReason = '';
        this.noteViewMode = false;
        this.noteId = null;
        this.showNoteForm = true;
    };

    cancelNote = () => { this.showNoteForm = false; this.noteViewMode = false; this.noteId = null; };

    saveNote = () => {
        if (!this.notePayment || !this.noteReason) {
            this.toastr.warning('Reason is required.'); return;
        }
        if (this.noteAmount <= 0) {
            this.toastr.warning('Amount must be positive.'); return;
        }
        if (this.noteType === 'credit' && this.noteAmount > +this.notePayment.amount!) {
            this.toastr.warning('Credit note cannot exceed original payment.'); return;
        }
        let payload = { Amount: this.noteAmount, Reason: this.noteReason };
        let req = this.noteType === 'credit'
            ? this.svc.creditNote(+this.notePayment.id, payload)
            : this.svc.debitNote(+this.notePayment.id, payload);
        req.subscribe({
            next: (saved: any) => {
                this.toastr.success(`${this.noteType === 'credit' ? 'Credit' : 'Debit'} note created.`);
                let newId = saved?.id || null;
                if (newId && this.noteApprovalWebpart?.hasPickerSelections()) {
                    this.noteId = newId;
                    this.noteApprovalWebpart.entityId = newId;
                    this.noteApprovalWebpart.submitSilently().then((r) => {
                        if (r) this.toastr.success('Submitted for approval.');
                        this.showNoteForm = false;
                        this.load();
                    });
                } else {
                    this.showNoteForm = false;
                    this.load();
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    };

    getPaymentTypeBadge = (t: any): string => {
        if (t == 1) return 'bg-warning text-dark';
        if (t == 2) return 'bg-danger';
        return 'bg-success';
    };

    getApprovalStatusLabel = (s: any): string => {
        let labels: any = {0: 'Draft', 1: 'Submitted', 2: 'Approved', 3: 'Rejected'};
        return labels[s] || 'Approved';
    };

    getApprovalStatusClass = (s: any): string => {
        let classes: any = {0: 'bg-secondary', 1: 'bg-warning text-dark', 2: 'bg-success', 3: 'bg-danger'};
        return classes[s] || 'bg-success';
    };

    submitNote = (p: Payment) => {
        Swal.fire({title: 'Submit for approval?', icon: 'question', showCancelButton: true, confirmButtonText: 'Submit'}).then((r) => {
            if (r.value) {
                this.svc.submitNote(+p.id).subscribe({
                    next: () => { this.toastr.success('Submitted for approval.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    approveNoteAction = (p: Payment) => {
        let msg = p.paymentType == 1 ? 'Approve credit note? This reverses the payment and reinstates the balance.' : 'Approve debit note? This adds the charge to the invoice.';
        Swal.fire({title: 'Approve note?', text: msg, icon: 'question', showCancelButton: true, confirmButtonText: 'Approve'}).then((r) => {
            if (r.value) {
                this.svc.approveNote(+p.id).subscribe({
                    next: () => { this.toastr.success('Note approved and applied.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    rejectNoteAction = (p: Payment) => {
        Swal.fire({
            title: 'Reject note?', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Reject', confirmButtonColor: '#d33',
            input: 'text', inputLabel: 'Reason for rejection', inputPlaceholder: 'Enter reason...',
            inputValidator: (value) => { if (!value) return 'Reason is required.'; return null; }
        }).then((r) => {
            if (r.value) {
                this.svc.rejectNote(+p.id, r.value).subscribe({
                    next: () => { this.toastr.success('Note rejected.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };

    getMethodLabel = (m: any): string => {
        return this.paymentMethods.find((p) => p.value === m)?.label || '';
    };
}
