import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {LoanAdvance} from '@/payroll/models/payroll-models';
import {LoanAdvanceService} from '@/payroll/services/payroll-services';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-payroll-loan-advances',
    templateUrl: './loan-advances.component.html'
})
export class PayrollLoanAdvancesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/loan-advances'], title: 'Loan Advances'}
    ];
    dashboardTitle = 'Payroll: Loan Advances';

    items: LoanAdvance[] = [];
    staffList: any[] = [];
    item: LoanAdvance = new LoanAdvance({status: 1});
    editMode: boolean = false;
    showForm: boolean = false;

    filterStaffId: any = null;
    filterStatus: any = null;

    statuses = [
        {value: 1, label: 'Active'},
        {value: 2, label: 'Fully Paid'},
        {value: 3, label: 'Cancelled'}
    ];

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: LoanAdvanceService,
        private staffSvc: StaffDetailsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/loanAdvances'),
            this.staffSvc.getBySearchDetails(Status.Active, null as any, null as any)
        ]).subscribe({
            next: ([items, staff]) => {
                this.items = items || [];
                this.staffList = (staff || []).sort((a: any, b: any) => (a.fullName || '').localeCompare(b.fullName || ''));
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load() {
        this.svc.get('/loanAdvances').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error)
        });
    }

    filtered(): LoanAdvance[] {
        return (this.items || []).filter((l) => {
            if (this.filterStaffId != null && l.staffDetailsId != this.filterStaffId) return false;
            if (this.filterStatus != null && l.status !== this.filterStatus) return false;
            return true;
        });
    }

    clearFilters() { this.filterStaffId = null; this.filterStatus = null; this.page = 1; }

    addNew() {
        let today = new Date().toISOString().substring(0, 10);
        this.item = new LoanAdvance({status: 1, balance: 0, principalAmount: 0, monthlyDeduction: 0, issueDate: today});
        this.editMode = false;
        this.showForm = true;
    }

    edit(x: LoanAdvance) {
        this.item = new LoanAdvance(x);
        this.item.issueDate = x.issueDate ? x.issueDate.substring(0, 10) : '';
        this.editMode = true;
        this.showForm = true;
    }

    cancel() { this.showForm = false; }

    save() {
        if (!this.item.staffDetailsId) { this.toastr.warning('Staff member is required.'); return; }
        if (!this.item.principalAmount || +this.item.principalAmount <= 0) { this.toastr.warning('Principal amount must be greater than 0.'); return; }
        if (!this.item.monthlyDeduction || +this.item.monthlyDeduction <= 0) { this.toastr.warning('Monthly deduction must be greater than 0.'); return; }
        let req = this.editMode ? this.svc.update('/loanAdvances', this.item) : this.svc.create('/loanAdvances', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(x: LoanAdvance) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/loanAdvances', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }

    getStaffName(staffId: any): string {
        let s = this.staffList.find((x: any) => x.id == staffId);
        return s ? (s.fullName || `${s.firstName || ''} ${s.lastName || ''}`.trim()) : '';
    }

    getStatusLabel(s: any): string {
        return this.statuses.find((x) => x.value === s)?.label || '';
    }

    getStatusBadgeClass(s: any): string {
        if (s === 1) return 'bg-success';
        if (s === 2) return 'bg-info';
        if (s === 3) return 'bg-danger';
        return 'bg-secondary';
    }
}
