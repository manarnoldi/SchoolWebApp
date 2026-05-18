import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {Payslip, PayrollPeriod} from '@/payroll/models/payroll-models';
import {PayrollPeriodService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-reports',
    templateUrl: './payroll-reports.component.html'
})
export class PayrollReportsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/reports'], title: 'Payroll Reports'}
    ];
    dashboardTitle = 'Payroll: Reports';

    periods: PayrollPeriod[] = [];
    selectedPeriodId: number | null = null;
    payslips: Payslip[] = [];
    reportType: 'muster' | 'statutory' | 'bank' = 'muster';

    page: number = 1;
    pageSize: number = 20;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: PayrollPeriodService
    ) {}

    ngOnInit(): void {
        this.svc.get('/payrollPeriods').subscribe({
            next: (data) => { this.periods = data || []; },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading periods.')
        });
    }

    loadReport = () => {
        if (!this.selectedPeriodId) {
            this.toastr.info('Select a payroll period first.');
            return;
        }
        this.svc.getPayslips(+this.selectedPeriodId).subscribe({
            next: (data) => {
                this.payslips = data || [];
                this.page = 1;
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading report.')
        });
    };

    getSelectedPeriodName = (): string => {
        let p = this.periods.find((x) => +x.id === +this.selectedPeriodId!);
        return p ? (p.name || '') : '';
    };

    // Muster Roll totals
    get totalBasic(): number { return (this.payslips || []).reduce((s, p) => s + (+p.basicSalary! || 0), 0); }
    get totalHouse(): number { return (this.payslips || []).reduce((s, p) => s + (+p.houseAllowance! || 0), 0); }
    get totalTransport(): number { return (this.payslips || []).reduce((s, p) => s + (+p.transportAllowance! || 0), 0); }
    get totalOtherAllow(): number { return (this.payslips || []).reduce((s, p) => s + (+p.otherAllowances! || 0), 0); }
    get totalGross(): number { return (this.payslips || []).reduce((s, p) => s + (+p.grossPay! || 0), 0); }
    get totalNssf(): number { return (this.payslips || []).reduce((s, p) => s + (+p.nssfEmployee! || 0), 0); }
    get totalNssfEmployer(): number { return (this.payslips || []).reduce((s, p) => s + (+p.nssfEmployer! || 0), 0); }
    get totalPaye(): number { return (this.payslips || []).reduce((s, p) => s + (+p.paye! || 0), 0); }
    get totalShif(): number { return (this.payslips || []).reduce((s, p) => s + (+p.shif! || 0), 0); }
    get totalAhl(): number { return (this.payslips || []).reduce((s, p) => s + (+p.ahl! || 0), 0); }
    get totalOtherDed(): number { return (this.payslips || []).reduce((s, p) => s + (+p.otherDeductions! || 0), 0); }
    get totalLoans(): number { return (this.payslips || []).reduce((s, p) => s + (+p.loanDeductions! || 0), 0); }
    get totalDeductions(): number { return (this.payslips || []).reduce((s, p) => s + (+p.totalDeductions! || 0), 0); }
    get totalNet(): number { return (this.payslips || []).reduce((s, p) => s + (+p.netPay! || 0), 0); }
}
