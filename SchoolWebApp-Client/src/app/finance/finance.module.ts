import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {SharedModule} from '@/shared/shared.module';
import {FinanceComponent} from './finance.component';
import {FinanceAccountsComponent} from './components/accounts/accounts.component';
import {FeeCategoriesComponent} from './components/fee-categories/fee-categories.component';
import {ExpenseCategoriesComponent} from './components/expense-categories/expense-categories.component';
import {ExpensesComponent} from './components/expenses/expenses.component';
import {JournalEntriesComponent} from './components/journal-entries/journal-entries.component';
import {PaymentsComponent} from './components/payments/payments.component';
import {FinanceInvoicesComponent} from './components/invoices/invoices.component';
import {FinanceReportsComponent} from './components/reports/finance-reports.component';
import {FinanceBudgetsComponent} from './components/budgets/budgets.component';
import {FinanceBudgetAmendmentsComponent} from './components/budget-amendments/budget-amendments.component';
import {SponsorsComponent} from './components/sponsors/sponsors.component';
import {SponsorshipsComponent} from './components/sponsorships/sponsorships.component';
import {SponsorPaymentsComponent} from './components/sponsor-payments/sponsor-payments.component';
import {FinanceBudgetMastersComponent} from './components/budget-masters/budget-masters.component';
import {FeeStructuresComponent} from './components/fee-structures/fee-structures.component';
import {ApprovalsModule} from '@/approvals/approvals.module';

@NgModule({
    declarations: [
        FinanceComponent,
        FinanceAccountsComponent,
        FeeCategoriesComponent,
        ExpenseCategoriesComponent,
        ExpensesComponent,
        JournalEntriesComponent,
        PaymentsComponent,
        FinanceInvoicesComponent,
        FinanceReportsComponent,
        FinanceBudgetsComponent,
        FinanceBudgetAmendmentsComponent,
        FinanceBudgetMastersComponent,
        FeeStructuresComponent,
        SponsorsComponent,
        SponsorshipsComponent,
        SponsorPaymentsComponent
    ],
    imports: [CommonModule, SharedModule, ApprovalsModule]
})
export class FinanceModule {}
