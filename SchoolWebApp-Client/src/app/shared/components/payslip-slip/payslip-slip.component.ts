import {Component, HostBinding, Input} from '@angular/core';
import {Payslip, PayslipLine} from '@/payroll/models/payroll-models';

/**
 * One printed payslip: earnings, how PAYE was worked out, deductions and net
 * pay. Used by Payroll > Processing and by a staff member's Pay tab, printed with
 * PAYSLIP_PRINT_CSS (the host is the .payslip box).
 */
@Component({
    selector: 'app-payslip-slip',
    templateUrl: './payslip-slip.component.html'
})
export class PayslipSlipComponent {
    @HostBinding('class') hostClass = 'payslip';
    @Input() p!: Payslip;
    @Input() schoolName = '';
    @Input() schoolLogo: string | null = null;
    @Input() employerKraPin = '';
    /** Used when the payslip carries no period name. */
    @Input() periodLabel = '';

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
}
