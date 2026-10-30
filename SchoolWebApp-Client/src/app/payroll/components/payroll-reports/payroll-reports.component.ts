import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {Payslip, PayrollPeriod, P9Employee, P9Row} from '@/payroll/models/payroll-models';
import {PayrollPeriodService} from '@/payroll/services/payroll-services';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {printInFrame, removePrintFrame} from '@/shared/utils/print-frame';
import {payrollReportPrintCss} from './payroll-report-print.styles';
import {P9CardComponent} from '@/shared/components/p9-card/p9-card.component';

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
    @ViewChild('p9Print') p9Print?: ElementRef<HTMLElement>;

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
    // The open tab, kept in the URL (?tab=) so a refresh or a shared link opens it.
    reportType: 'muster' | 'statutory' | 'bank' | 'payslips' | 'p9' = 'muster';
    // The period the loaded report is for - the Payslips tab prints its slips.
    loadedPeriod: PayrollPeriod | null = null;
    loading = false;

    // P9A tax deduction cards: a year's pay and PAYE per employee, from approved payroll.
    p9Years: number[] = [];
    p9Year: number | null = null;
    p9Employees: P9Employee[] = [];
    p9StaffId: number | null = null;
    p9Loaded = false;
    p9Loading = false;
    // The cards being printed: the one on screen, or everyone.
    p9Printing: P9Employee[] = [];
    readonly monthNames = P9CardComponent.MONTHS;
    readonly p9Cols = P9CardComponent.COLUMNS;

    page: number = 1;
    pageSize: number = 15;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: PayrollPeriodService,
        private schoolSvc: SchoolDetailsService,
        private globalSettingSvc: GlobalSettingService,
        private route: ActivatedRoute,
        private router: Router
    ) {}

    ngOnInit(): void {
        let tab = this.route.snapshot.queryParamMap.get('tab');
        if (tab === 'muster' || tab === 'statutory' || tab === 'bank' || tab === 'payslips' || tab === 'p9') this.reportType = tab;
        // A link from Processing names the period to open.
        let fromLink = +(this.route.snapshot.queryParamMap.get('periodId') || 0);
        this.svc.get('/payrollPeriods').subscribe({
            // Reports only cover payroll that has been signed off. A Draft or
            // Processed period can still be re-run, so its figures are not final
            // and must not be reported on. Status 2 = Approved, 3 = Posted.
            next: (data) => {
                this.periods = (data || []).filter((p) => p.status === 2 || p.status === 3);
                this.p9Years = [...new Set(this.periods.map((p) => +p.year!))].sort((a, b) => b - a);
                this.p9Year = this.p9Years[0] ?? null;
                // The period named by a link, otherwise the latest approved one - loaded straight away.
                let latest = [...this.periods].sort((a, b) => (+b.year! * 100 + +b.month!) - (+a.year! * 100 + +a.month!))[0];
                this.selectedPeriodId = fromLink && this.periods.some((p) => +p.id === fromLink) ? fromLink : (latest ? +latest.id : null);
                if (this.selectedPeriodId) this.loadReport();
                if (this.reportType === 'p9') this.loadP9();
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

    setTab = (tab: 'muster' | 'statutory' | 'bank' | 'payslips' | 'p9') => {
        this.reportType = tab;
        this.router.navigate([], {relativeTo: this.route, queryParams: {tab}, queryParamsHandling: 'merge', replaceUrl: true});
        if (tab === 'p9' && !this.p9Loaded) this.loadP9();
    };

    // --- P9 ---
    p9YearChanged = () => {
        this.p9Employees = [];
        this.p9Loaded = false;
        this.loadP9();
    };

    loadP9 = () => {
        if (!this.p9Year) return;
        this.p9Loading = true;
        this.svc.getP9(this.p9Year).subscribe({
            next: (r) => {
                this.p9Employees = r?.employees || [];
                this.p9Loaded = true;
                this.p9Loading = false;
                if (!this.p9Employees.some((e) => e.staffId === this.p9StaffId))
                    this.p9StaffId = this.p9Employees[0]?.staffId ?? null;
            },
            error: (err) => {
                this.p9Loading = false;
                this.toastr.error(err.error?.message || 'Error loading the P9 cards.');
            }
        });
    };

    p9Selected = (): P9Employee | null => this.p9Employees.find((e) => e.staffId === +this.p9StaffId!) || null;

    // All twelve months, as on the form; a month with no approved payslip is left blank.
    p9Months = (e: P9Employee): {name: string; row: P9Row | null}[] =>
        this.monthNames.map((name, i) => ({name, row: e.rows.find((r) => r.month === i + 1) || null}));

    p9Total = (e: P9Employee, key: keyof P9Row): number => e.rows.reduce((s, r) => s + (+r[key] || 0), 0);

    printP9 = (all: boolean) => {
        let one = this.p9Selected();
        this.p9Printing = all ? this.p9Employees : (one ? [one] : []);
        if (this.p9Printing.length === 0) return;
        this.printedOn = new Date();
        // Let Angular render the chosen cards before copying them.
        setTimeout(() => {
            let html = this.p9Print?.nativeElement.innerHTML || '';
            let title = all ? `P9 Tax Cards ${this.p9Year}` : `P9 ${this.p9Year} - ${one!.name}`;
            if (html.trim()) printInFrame(title, payrollReportPrintCss('landscape'), html, 'landscape');
        });
    };

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
    // Levies the school pays itself (NITA where it bears it), by type.
    get employerLevies(): {name: string; amount: number}[] {
        let byName = new Map<string, number>();
        for (let p of this.payslips || [])
            for (let l of p.employerCostLines || [])
                byName.set(l.name || 'Levy', (byName.get(l.name || 'Levy') || 0) + (+l.amount! || 0));
        return [...byName].map(([name, amount]) => ({name, amount}));
    }
    get totalEmployerLevies(): number { return this.employerLevies.reduce((s, l) => s + l.amount, 0); }
    get totalStatutory(): number {
        return this.totalPaye + this.totalNssfRemittance + this.totalShif + this.totalAhlRemittance + this.totalEmployerLevies;
    }

    // A different period: clear the old figures and load the new ones.
    periodChanged = () => {
        this.payslips = [];
        this.loadedPeriod = null;
        this.loadReport();
    };

    loadReport = () => {
        if (!this.selectedPeriodId) return;
        this.loading = true;
        this.svc.getPayslips(+this.selectedPeriodId).subscribe({
            next: (data) => {
                this.payslips = data || [];
                this.page = 1;
                this.loadedPeriod = this.periods.find((p) => +p.id === +this.selectedPeriodId!) || null;
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.error?.message || 'Error loading report.');
            }
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
