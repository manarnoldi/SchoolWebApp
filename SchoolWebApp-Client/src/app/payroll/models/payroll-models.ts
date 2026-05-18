import {ResourceModel} from '@/core/models/ResourceModel';

export class EarningType extends ResourceModel<EarningType> {
    public name?: string;
    public code?: string;
    public isTaxable?: boolean;
    public isActive?: boolean;
    public description?: string;
    constructor(m?: Partial<EarningType>) { super(m); }
}

export class DeductionType extends ResourceModel<DeductionType> {
    public name?: string;
    public code?: string;
    public isStatutory?: boolean;
    public isActive?: boolean;
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
}

export class Payslip extends ResourceModel<Payslip> {
    public payrollPeriodId?: number;
    public staffDetailsId?: number;
    public staffName?: string;
    public staffUpi?: string;
    public kraPin?: string;
    public nssfNumber?: string;
    public designationName?: string;
    public departmentName?: string;
    public bankName?: string;
    public bankAccountNumber?: string;
    public basicSalary?: number;
    public houseAllowance?: number;
    public transportAllowance?: number;
    public otherAllowances?: number;
    public grossPay?: number;
    public nssfEmployee?: number;
    public taxableIncome?: number;
    public grossTax?: number;
    public personalRelief?: number;
    public insuranceRelief?: number;
    public paye?: number;
    public shif?: number;
    public ahl?: number;
    public nssfEmployer?: number;
    public otherDeductions?: number;
    public loanDeductions?: number;
    public totalDeductions?: number;
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
