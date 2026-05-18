import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {SharedModule} from '@/shared/shared.module';
import {PayrollComponent} from './payroll.component';
import {PayrollEarningTypesComponent} from './components/earning-types/earning-types.component';
import {PayrollDeductionTypesComponent} from './components/deduction-types/deduction-types.component';
import {PayrollTaxBandsComponent} from './components/tax-bands/tax-bands.component';
import {PayrollSettingsComponent} from './components/payroll-settings/payroll-settings.component';
import {PayrollEmployeeSalariesComponent} from './components/employee-salaries/employee-salaries.component';
import {PayrollLoanAdvancesComponent} from './components/loan-advances/loan-advances.component';
import {PayrollPeriodsComponent} from './components/payroll-periods/payroll-periods.component';
import {PayrollReportsComponent} from './components/payroll-reports/payroll-reports.component';

@NgModule({
    declarations: [
        PayrollComponent,
        PayrollEarningTypesComponent,
        PayrollDeductionTypesComponent,
        PayrollTaxBandsComponent,
        PayrollSettingsComponent,
        PayrollEmployeeSalariesComponent,
        PayrollLoanAdvancesComponent,
        PayrollPeriodsComponent,
        PayrollReportsComponent
    ],
    imports: [CommonModule, SharedModule]
})
export class PayrollModule {}
