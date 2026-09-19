import {Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, SimpleChanges, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {Payslip, PayrollPeriod, PayslipLine} from '@/payroll/models/payroll-models';
import {PayrollPeriodService} from '@/payroll/services/payroll-services';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {printInFrame, removePrintFrame} from '@/shared/utils/print-frame';
import {PAYSLIP_PRINT_CSS} from './payslip-print.styles';

// The payslips of one payroll period: list, search, statutory-number fixes and
// printing. It sits under the periods table on the Processing page, which
// passes in the period chosen there - so processing, checking the slips,
// approving and printing all happen in one place.
@Component({
    selector: 'app-payroll-payslips',
    templateUrl: './payslips.component.html',
    styleUrls: ['./payslips.component.scss']
})
export class PayrollPayslipsComponent implements OnInit, OnChanges, OnDestroy {
    // The rendered slips, copied into the print frame by runPrint().
    @ViewChild('printArea') printArea?: ElementRef<HTMLElement>;

    // The period whose payslips are shown; null until one is chosen.
    @Input() period: PayrollPeriod | null = null;
    // Bumped by the Processing page after a (re-)process, so the slips reload
    // even though the same period stays selected.
    @Input() reloadToken = 0;

    payslips: Payslip[] = [];

    searchText = '';

    // Header details for the printed payslip.
    schoolName = '';
    schoolLogo: string | null = null;
    employerKraPin = '';

    loading = false;
    // The single payslip opened for preview/print; null when printing the batch.
    printOne: Payslip | null = null;
    printAll = false;

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    // Editing the statutory numbers that print on the payslip. Deliberately
    // limited to these four - the rest of the staff record is managed on the
    // Staff Details page.
    showIdentifiers = false;
    savingIdentifiers = false;
    identifiersFor: Payslip | null = null;
    identifiers = {kraPinNo: '', idNumber: '', nssfNo: '', nhifNo: '', excludeFromPayroll: false};

    constructor(
        private toastr: ToastrService,
        private svc: PayrollPeriodService,
        private schoolSvc: SchoolDetailsService,
        private staffSvc: StaffDetailsService,
        private globalSettingSvc: GlobalSettingService
    ) {}

    ngOnInit(): void {
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
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the school details for the payslip header.')
        });
    }

    ngOnChanges(changes: SimpleChanges): void {
        // A different period starts from a clean list; a reload of the same
        // period (after re-processing) keeps the search and page.
        let periodChanged = changes['period'] &&
            +(changes['period'].previousValue?.id || 0) !== +(this.period?.id || 0);
        if (periodChanged) this.searchText = '';
        if (periodChanged || changes['reloadToken']) this.loadPayslips(!periodChanged);
    }

    selectedPeriod(): PayrollPeriod | undefined {
        return this.period || undefined;
    }

    periodLabel(): string {
        return this.period?.name || '';
    }

    // keepPage is used after an inline edit, so the user is not bounced back to
    // page 1 when a reload refreshes the row they just changed.
    loadPayslips(keepPage = false): void {
        let period = this.selectedPeriod();
        if (!keepPage) this.page = 1;
        if (!period || !period.id) { this.payslips = []; return; }
        this.loading = true;
        this.svc.getPayslips(+period.id).subscribe({
            next: (data) => { this.payslips = data || []; this.loading = false; },
            error: (err) => {
                this.loading = false;
                this.payslips = [];
                this.toastr.error(err.error?.message || 'Error loading payslips.');
            }
        });
    }

    clearFilters(): void {
        this.searchText = '';
        this.page = 1;
    }

    filtered(): Payslip[] {
        let q = (this.searchText || '').trim().toLowerCase();
        if (!q) return this.payslips;
        return this.payslips.filter((p) =>
            (p.staffName || '').toLowerCase().includes(q) ||
            (p.staffUpi || '').toLowerCase().includes(q));
    }

    // --- Totals for the list footer ---
    totalGross = (): number => this.filtered().reduce((s, p) => s + (+p.grossPay! || 0), 0);
    totalDeductions = (): number => this.filtered().reduce((s, p) => s + (+p.totalDeductions! || 0), 0);
    totalNet = (): number => this.filtered().reduce((s, p) => s + (+p.netPay! || 0), 0);
    totalPaye = (): number => this.filtered().reduce((s, p) => s + (+p.paye! || 0), 0);

    // --- Payslip breakdown helpers ---
    // "Other Allowances" on the printed slip means everything on top of basic:
    // the allowance columns plus any additional earning lines. BASIC is already
    // shown on its own row, so it is excluded here.
    otherAllowanceLines(p: Payslip): {name: string; amount: number; taxable: boolean}[] {
        let lines: {name: string; amount: number; taxable: boolean}[] = [];
        if (+p.houseAllowance! > 0) lines.push({name: 'House Allowance', amount: +p.houseAllowance!, taxable: true});
        if (+p.transportAllowance! > 0) lines.push({name: 'Transport Allowance', amount: +p.transportAllowance!, taxable: true});
        if (+p.otherAllowances! > 0) lines.push({name: 'Other Allowances', amount: +p.otherAllowances!, taxable: true});
        for (let e of (p.earnings || []) as PayslipLine[]) {
            if (e.code === 'BASIC' || e.code === 'HSEALL' || e.code === 'TRNALL') continue;
            if (+e.amount! > 0) lines.push({name: e.name || 'Earning', amount: +e.amount!, taxable: e.isTaxable !== false});
        }
        return lines;
    }

    // Earnings left out of taxable pay - the gap between gross and taxable pay.
    nonTaxableEarnings(p: Payslip): number {
        return Math.round(((+p.grossPay! || 0) - (+p.taxablePay! || 0)) * 100) / 100;
    }

    // Gross less deductions before net pay is rounded, so the payslip can show the
    // rounding as its own line and still add up.
    netBeforeRounding(p: Payslip): number {
        return Math.round(((+p.grossPay! || 0) - (+p.totalDeductions! || 0)) * 100) / 100;
    }

    hasRounding(p: Payslip): boolean {
        return Math.abs(+p.roundingAdjustment! || 0) >= 0.005;
    }

    // Deduction lines other than the statutory ones already printed on their own
    // rows (PAYE, NSSF, SHIF, AHL) - voluntary items and loan instalments.
    otherDeductionLines(p: Payslip): {name: string; amount: number}[] {
        let statutory = ['PAYE', 'NSSF', 'SHIF', 'AHL', 'NSSFER'];
        return ((p.deductions || []) as PayslipLine[])
            .filter((d) => !statutory.includes(d.code || '') && +d.amount! > 0)
            .map((d) => ({name: d.name || 'Deduction', amount: +d.amount!}));
    }

    // --- Payslip identifiers (KRA PIN, ID, NSSF, SHA) ---
    // These print from the staff record rather than being copied onto the
    // payslip, so a correction shows up immediately - no re-processing needed.
    // An excluded staff member will not be paid, so blank statutory numbers are
    // not a problem for them and are not reported as missing.
    missingIdentifiers(p: Payslip): string[] {
        if (p.excludeFromPayroll) return [];
        let missing: string[] = [];
        if (!p.kraPin) missing.push('KRA PIN');
        if (!p.idNumber) missing.push('ID No');
        if (!p.nssfNumber) missing.push('NSSF No');
        if (!p.shaNumber) missing.push('SHA No');
        return missing;
    }

    incompleteCount(): number {
        return this.filtered().filter((p) => this.missingIdentifiers(p).length > 0).length;
    }

    excludedCount(): number {
        return this.filtered().filter((p) => p.excludeFromPayroll).length;
    }

    editIdentifiers(p: Payslip): void {
        this.identifiersFor = p;
        this.identifiers = {
            kraPinNo: p.kraPin || '',
            idNumber: p.idNumber || '',
            nssfNo: p.nssfNumber || '',
            nhifNo: p.shaNumber || '',
            excludeFromPayroll: !!p.excludeFromPayroll
        };
        this.showIdentifiers = true;
    }

    cancelIdentifiers(): void {
        this.showIdentifiers = false;
        this.identifiersFor = null;
    }

    saveIdentifiers(): void {
        let target = this.identifiersFor;
        if (!target) return;
        this.savingIdentifiers = true;
        this.staffSvc.updatePayrollDetails(+target.staffDetailsId!, {
            kraPinNo: this.identifiers.kraPinNo?.trim() || null,
            idNumber: this.identifiers.idNumber?.trim() || null,
            nssfNo: this.identifiers.nssfNo?.trim() || null,
            nhifNo: this.identifiers.nhifNo?.trim() || null,
            excludeFromPayroll: !!this.identifiers.excludeFromPayroll
        }).subscribe({
            next: () => {
                this.savingIdentifiers = false;
                this.toastr.success(`Details updated for ${target!.staffName}.`);
                this.showIdentifiers = false;
                this.identifiersFor = null;
                // Reload so every payslip for this staff member picks up the change,
                // not just the row that was edited.
                this.loadPayslips(true);
            },
            error: (err) => {
                this.savingIdentifiers = false;
                this.toastr.error(err.error?.message || err.error || 'Error saving details.');
            }
        });
    }

    // --- Printing ---
    // Both paths render into the same hidden print area, which runPrint() then
    // prints on its own.
    // Payslips that may actually be issued. Someone taken off the payroll should
    // not be handed a slip, so they are left out of printing even though their
    // row still shows the payroll that was run before they were excluded.
    printable(): Payslip[] {
        return this.filtered().filter((p) => !p.excludeFromPayroll);
    }

    print(p: Payslip): void {
        if (p.excludeFromPayroll) {
            this.toastr.warning(`${p.staffName} is excluded from payroll.`);
            return;
        }
        this.printOne = p;
        this.printAll = false;
        this.runPrint();
    }

    printAllPayslips(): void {
        if (this.printable().length === 0) { this.toastr.info('No payslips to print.'); return; }
        this.printOne = null;
        this.printAll = true;
        this.runPrint();
    }

    // The rendered slips are printed from their own frame with their own
    // stylesheet (see printInFrame), so the app's print rules cannot move,
    // shrink or cover them.
    private runPrint(): void {
        // Let Angular render the slips before copying them.
        setTimeout(() => {
            let slips = this.printArea?.nativeElement.innerHTML || '';
            this.printOne = null;
            this.printAll = false;
            if (slips.trim()) printInFrame(`Payslips - ${this.periodLabel()}`, PAYSLIP_PRINT_CSS, slips);
        }, 150);
    }

    ngOnDestroy(): void {
        removePrintFrame();
    }

    // The payslips fed to the print area: one, or every printable filtered row.
    printList(): Payslip[] {
        if (this.printOne) return [this.printOne];
        if (this.printAll) return this.printable();
        return [];
    }

    // The same slips in twos, one pair per row across the A4 sheet.
    printPairs(): Payslip[][] {
        const list = this.printList();
        const pairs: Payslip[][] = [];
        for (let i = 0; i < list.length; i += 2) pairs.push(list.slice(i, i + 2));
        return pairs;
    }

    // The pairs are rebuilt on every change-detection pass; tracking by position
    // stops Angular re-creating the slips' DOM each time.
    trackByIndex(index: number): number {
        return index;
    }
}
