import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {Payslip, PayrollPeriod} from '@/payroll/models/payroll-models';
import {PayrollPeriodService} from '@/payroll/services/payroll-services';

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
    item: any = {month: null, year: new Date().getFullYear()};
    showForm: boolean = false;

    payslips: Payslip[] = [];
    selectedPeriod: PayrollPeriod | null = null;
    showPayslips: boolean = false;

    selectedPayslip: Payslip | null = null;
    showPayslipDetail: boolean = false;

    page: number = 1;
    pageSize: number = 10;
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
        private svc: PayrollPeriodService
    ) {}

    ngOnInit(): void {
        this.load();
    }

    load = () => {
        this.svc.get('/payrollPeriods').subscribe({
            next: (data) => { this.periods = data || []; },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading periods.')
        });
    };

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
        Swal.fire({
            title: 'Process payroll?',
            text: `Process payroll for ${p.name}?`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Process'
        }).then((r) => {
            if (r.value) {
                this.svc.process(+p.id).subscribe({
                    next: () => {
                        this.toastr.success('Payroll processed successfully.');
                        this.load();
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

    viewPayslips = (p: PayrollPeriod) => {
        this.svc.getPayslips(+p.id).subscribe({
            next: (data) => {
                this.payslips = data || [];
                this.selectedPeriod = p;
                this.showPayslips = true;
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading payslips.')
        });
    };

    closePayslips = () => {
        this.showPayslips = false;
        this.selectedPeriod = null;
        this.payslips = [];
    };

    viewPayslip = (ps: Payslip) => {
        this.svc.getPayslip(+ps.id).subscribe({
            next: (data) => {
                this.selectedPayslip = data;
                this.showPayslipDetail = true;
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading payslip.')
        });
    };

    closePayslipDetail = () => {
        this.showPayslipDetail = false;
        this.selectedPayslip = null;
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

    getStatusLabel = (s: number): string => {
        if (s === 0) return 'Draft';
        if (s === 1) return 'Processed';
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

    getPayslipsTotalGross = (): number => {
        return (this.payslips || []).reduce((s, p) => s + (+p.grossPay! || 0), 0);
    };

    getPayslipsTotalNssf = (): number => {
        return (this.payslips || []).reduce((s, p) => s + (+p.nssfEmployee! || 0), 0);
    };

    getPayslipsTotalPaye = (): number => {
        return (this.payslips || []).reduce((s, p) => s + (+p.paye! || 0), 0);
    };

    getPayslipsTotalShif = (): number => {
        return (this.payslips || []).reduce((s, p) => s + (+p.shif! || 0), 0);
    };

    getPayslipsTotalAhl = (): number => {
        return (this.payslips || []).reduce((s, p) => s + (+p.ahl! || 0), 0);
    };

    getPayslipsTotalNet = (): number => {
        return (this.payslips || []).reduce((s, p) => s + (+p.netPay! || 0), 0);
    };
}
