import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {Payslip, PayrollPeriod} from '@/payroll/models/payroll-models';
import {PayrollPeriodService} from '@/payroll/services/payroll-services';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {printInFrame, removePrintFrame} from '@/shared/utils/print-frame';
import {payrollReportPrintCss} from './payroll-report-print.styles';

@Component({
    selector: 'app-payroll-reports',
    templateUrl: './payroll-reports.component.html'
})
export class PayrollReportsComponent implements OnInit, OnDestroy {
    // The printable reports, rendered hidden and printed from their own frame.
    // The muster roll lists every employee, not just the page on screen.
    @ViewChild('musterPrint') musterPrint?: ElementRef<HTMLElement>;
    @ViewChild('statutoryPrint') statutoryPrint?: ElementRef<HTMLElement>;
    @ViewChild('bankPrint') bankPrint?: ElementRef<HTMLElement>;

    // Header details for the printout.
    schoolName = '';
    schoolLogo: string | null = null;
    employerKraPin = '';
    printedOn = new Date();
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
        private svc: PayrollPeriodService,
        private schoolSvc: SchoolDetailsService,
        private globalSettingSvc: GlobalSettingService
    ) {}

    ngOnInit(): void {
        this.svc.get('/payrollPeriods').subscribe({
            // Reports only cover payroll that has been signed off. A Draft or
            // Processed period can still be re-run, so its figures are not final
            // and must not be reported on. Status 2 = Approved, 3 = Posted.
            next: (data) => {
                this.periods = (data || []).filter((p) => p.status === 2 || p.status === 3);
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading periods.')
        });
        forkJoin([
            this.schoolSvc.get('/schoolDetails'),
            this.globalSettingSvc.getByKey('Payroll', 'EmployerKraPin')
        ]).subscribe({
            next: ([school, kraPin]) => {
                let s: any = (school || [])[0];
                this.schoolName = s?.name || '';
                this.schoolLogo = s?.logoAsBase64 || null;
                this.employerKraPin = kraPin?.settingValue || '';
            },
            error: () => {}
        });
    }

    ngOnDestroy(): void {
        removePrintFrame();
    }

    printMusterRoll = () => {
        this.printReport(this.musterPrint, 'Muster Roll', 'landscape');
    };

    printStatutoryReturns = () => {
        this.printReport(this.statutoryPrint, 'Statutory Returns', 'portrait');
    };

    printBankSchedule = () => {
        this.printReport(this.bankPrint, 'Bank Schedule', 'portrait');
    };

    private printReport(source: ElementRef<HTMLElement> | undefined, name: string,
                        orientation: 'portrait' | 'landscape'): void {
        if (this.payslips.length === 0) return;
        this.printedOn = new Date();
        // Let Angular refresh the printed date before copying the report.
        setTimeout(() => {
            let html = source?.nativeElement.innerHTML || '';
            if (html.trim())
                printInFrame(`${name} - ${this.getSelectedPeriodName()}`, payrollReportPrintCss(orientation), html, orientation);
        });
    }

    get totalNssfRemittance(): number { return this.totalNssf + this.totalNssfEmployer; }
    // Housing Levy is remitted as the employee's 1.5% plus the employer's match.
    get totalAhlRemittance(): number { return this.totalAhl + this.totalAhlEmployer; }
    get totalStatutory(): number {
        return this.totalPaye + this.totalNssfRemittance + this.totalShif + this.totalAhlRemittance;
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
    get totalAhlEmployer(): number { return (this.payslips || []).reduce((s, p) => s + (+p.ahlEmployer! || 0), 0); }
    get totalOtherDed(): number { return (this.payslips || []).reduce((s, p) => s + (+p.otherDeductions! || 0), 0); }
    get totalLoans(): number { return (this.payslips || []).reduce((s, p) => s + (+p.loanDeductions! || 0), 0); }
    get totalDeductions(): number { return (this.payslips || []).reduce((s, p) => s + (+p.totalDeductions! || 0), 0); }
    get totalNet(): number { return (this.payslips || []).reduce((s, p) => s + (+p.netPay! || 0), 0); }
}
