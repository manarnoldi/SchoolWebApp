import {ResourceModel} from '@/core/models/ResourceModel';

// How an earning or deduction applied from its type is worked out.
// Deduction percentages are of gross pay; earning percentages are of basic.
export enum PayrollCalculationMethod {
    FixedAmount = 0,
    Percentage = 1
}

export class EarningType extends ResourceModel<EarningType> {
    public name?: string;
    public code?: string;
    public isTaxable?: boolean;
    public isActive?: boolean;
    public calculationMethod?: number;
    public defaultValue?: number | null;
    public appliesToAll?: boolean;
    // Payroll accounts for this itself, so it is never applied from the type.
    public isSystemComputed?: boolean;
    public description?: string;
    constructor(m?: Partial<EarningType>) { super(m); }
}

export class DeductionType extends ResourceModel<DeductionType> {
    public name?: string;
    public code?: string;
    public isStatutory?: boolean;
    public isActive?: boolean;
    public calculationMethod?: number;
    public defaultValue?: number | null;
    public appliesToAll?: boolean;
    // Payroll computes this itself, so it is never applied from the type.
    public isSystemComputed?: boolean;
    // How payroll treats the deduction for tax. Configured per type so a change
    // in the law is a settings change rather than a code change.
    public isTaxDeductible?: boolean;
    public taxDeductibleCap?: number | null;
    // Pooled with NSSF under the retirement relief limits.
    public isRetirementContribution?: boolean;
    // Liability account credited when payroll posts to the GL; null uses the general one.
    public liabilityAccountId?: number | null;
    public description?: string;
    constructor(m?: Partial<DeductionType>) { super(m); }
}

export class TaxBand extends ResourceModel<TaxBand> {
    public description?: string;
    public lowerLimit?: number;
    public upperLimit?: number;
    public rate?: number;
    public effectiveDate?: string;
    public isActive?: boolean;
    constructor(m?: Partial<TaxBand>) { super(m); }
}

// One NSSF tier as it stood from a given date. Every tier sharing an effective
// date forms the set payroll uses for a period on or after that date.
export class NssfBand extends ResourceModel<NssfBand> {
    public name?: string;
    public tier?: number;
    public lowerLimit?: number;
    public upperLimit?: number;
    public rate?: number;
    public effectiveDate?: string;
    public isActive?: boolean;
    public description?: string;
    constructor(m?: Partial<NssfBand>) { super(m); }
}

// How a relief's value is arrived at.
export enum ReliefBasis {
    FixedAmount = 0,
    PercentageOfDeduction = 1
}

// A relief comes off the tax itself, after the bands have been applied.
export class PayrollRelief extends ResourceModel<PayrollRelief> {
    public name?: string;
    public code?: string;
    public basis?: number;
    public deductionTypeId?: number | null;
    public deductionTypeName?: string;
    public value?: number;
    public monthlyCap?: number | null;
    public appliesToAll?: boolean;
    public isActive?: boolean;
    public description?: string;
    constructor(m?: Partial<PayrollRelief>) { super(m); }
}

export class PayrollSetting extends ResourceModel<PayrollSetting> {
    public key?: string;
    public name?: string;
    public value?: number;
    public category?: string;
    public description?: string;
    public effectiveDate?: string;
    public isActive?: boolean;
    constructor(m?: Partial<PayrollSetting>) { super(m); }
}

export class EmployeeSalaryItem {
    public id?: number;
    public earningTypeId?: number;
    public earningTypeName?: string;
    public deductionTypeId?: number;
    public deductionTypeName?: string;
    public amount?: number;
    // Which table the line belongs to. A newly added line has no type chosen yet,
    // so it cannot be told apart by earningTypeId/deductionTypeId alone. Client
    // side only - stripped before the line is sent to the API.
    public kind?: 'earning' | 'deduction';
}

export class EmployeeSalary extends ResourceModel<EmployeeSalary> {
    public staffDetailsId?: number;
    public staffName?: string;
    public staffUpi?: string;
    public basicSalary?: number;
    public houseAllowance?: number;
    public transportAllowance?: number;
    public otherAllowances?: number;
    public effectiveDate?: string;
    public isActive?: boolean;
    public notes?: string;
    public totalEarnings?: number;
    public items!: EmployeeSalaryItem[];
    constructor(m?: Partial<EmployeeSalary>) { super(m); if (!this.items) this.items = []; }
}

export class LoanAdvance extends ResourceModel<LoanAdvance> {
    public staffDetailsId?: number;
    public staffName?: string;
    public description?: string;
    public principalAmount?: number;
    public monthlyDeduction?: number;
    public balance?: number;
    public issueDate?: string;
    public status?: number;
    public notes?: string;
    constructor(m?: Partial<LoanAdvance>) { super(m); }
}

export class PayslipLine {
    public id?: number;
    public name?: string;
    public code?: string;
    public amount?: number;
    // Earning lines only: false where the earning is left out of taxable pay.
    public isTaxable?: boolean;
}

export class Payslip extends ResourceModel<Payslip> {
    public payrollPeriodId?: number;
    public periodName?: string;
    public staffDetailsId?: number;
    public staffName?: string;
    public staffUpi?: string;
    public kraPin?: string;
    public nssfNumber?: string;
    public idNumber?: string;
    public shaNumber?: string;
    public excludeFromPayroll?: boolean;
    public designationName?: string;
    public departmentName?: string;
    public bankName?: string;
    public bankAccountNumber?: string;
    public basicSalary?: number;
    public houseAllowance?: number;
    public transportAllowance?: number;
    public otherAllowances?: number;
    public grossPay?: number;
    public nssfTier1?: number;
    public nssfTier2?: number;
    public nssfEmployee?: number;
    // The PAYE working, so the payslip can show how taxable income was reached.
    public taxablePay?: number;
    public retirementRelief?: number;
    public otherTaxDeductible?: number;
    public taxableIncome?: number;
    public grossTax?: number;
    public personalRelief?: number;
    public insuranceRelief?: number;
    public paye?: number;
    public shif?: number;
    public ahl?: number;
    public nssfEmployer?: number;
    // Employer Housing Levy - for remittance reports only, never the payslip.
    public ahlEmployer?: number;
    public otherDeductions?: number;
    public loanDeductions?: number;
    public totalDeductions?: number;
    public roundingAdjustment?: number;
    public netPay?: number;
    public earnings!: PayslipLine[];
    public deductions!: PayslipLine[];
    constructor(m?: Partial<Payslip>) { super(m); if (!this.earnings) this.earnings = []; if (!this.deductions) this.deductions = []; }
}

export class PayrollPeriod extends ResourceModel<PayrollPeriod> {
    public month?: number;
    public year?: number;
    public name?: string;
    public status?: number;
    public statusLabel?: string;
    public processedDate?: string;
    public approvedDate?: string;
    public postedDate?: string;
    public payslipCount?: number;
    public totalGross?: number;
    public totalNet?: number;
    public totalPaye?: number;
    public totalNssf?: number;
    public totalShif?: number;
    constructor(m?: Partial<PayrollPeriod>) { super(m); }
}
