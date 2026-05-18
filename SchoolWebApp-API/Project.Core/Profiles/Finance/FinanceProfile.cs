using AutoMapper;
using SchoolWebApp.Core.DTOs.Finance.Account;
using SchoolWebApp.Core.DTOs.Finance.Budget;
using SchoolWebApp.Core.DTOs.Finance.BudgetAmendment;
using SchoolWebApp.Core.DTOs.Finance.BudgetMaster;
using SchoolWebApp.Core.DTOs.Finance.Expense;
using SchoolWebApp.Core.DTOs.Finance.ExpenseCategory;
using SchoolWebApp.Core.DTOs.Finance.FeeCategory;
using SchoolWebApp.Core.DTOs.Finance.FeeStructure;
using SchoolWebApp.Core.DTOs.Finance.JournalEntry;
using SchoolWebApp.Core.DTOs.Finance.Payment;
using SchoolWebApp.Core.DTOs.Finance.StudentInvoice;
using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.Core.Profiles.Finance
{
    public class FinanceProfile : Profile
    {
        public FinanceProfile()
        {
            // Account
            CreateMap<Account, AccountDto>()
                .ForMember(d => d.ParentAccountName, o => o.MapFrom(s => s.ParentAccount != null ? s.ParentAccount.Name : null));
            CreateMap<CreateAccountDto, Account>();
            CreateMap<AccountDto, Account>();

            // Fee Category
            CreateMap<FeeCategory, FeeCategoryDto>()
                .ForMember(d => d.IncomeAccountName, o => o.MapFrom(s => s.IncomeAccount != null ? s.IncomeAccount.Name : null));
            CreateMap<CreateFeeCategoryDto, FeeCategory>();
            CreateMap<FeeCategoryDto, FeeCategory>();

            // Fee Structure
            CreateMap<FeeStructure, FeeStructureDto>()
                .ForMember(d => d.AcademicYearName, o => o.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
                .ForMember(d => d.SessionName, o => o.MapFrom(s => s.Session != null ? s.Session.SessionName : null))
                .ForMember(d => d.LearningLevelName, o => o.MapFrom(s => s.LearningLevel != null ? s.LearningLevel.Name : null))
                .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.Items.Sum(i => i.Amount)))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
            CreateMap<CreateFeeStructureDto, FeeStructure>();

            CreateMap<FeeStructureItem, FeeStructureItemDto>()
                .ForMember(d => d.FeeCategoryName, o => o.MapFrom(s => s.FeeCategory != null ? s.FeeCategory.Name : null));
            CreateMap<CreateFeeStructureItemDto, FeeStructureItem>();

            // Invoice
            CreateMap<StudentInvoice, StudentInvoiceDto>()
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student != null ? s.Student.FullName : null))
                .ForMember(d => d.StudentUPI, o => o.MapFrom(s => s.Student != null ? s.Student.UPI : null))
                .ForMember(d => d.AcademicYearName, o => o.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
                .ForMember(d => d.SessionName, o => o.MapFrom(s => s.Session != null ? s.Session.SessionName : null))
                .ForMember(d => d.Balance, o => o.MapFrom(s => s.TotalAmount - s.PaidAmount - s.DiscountAmount));
            CreateMap<CreateStudentInvoiceDto, StudentInvoice>();

            CreateMap<StudentInvoiceItem, StudentInvoiceItemDto>()
                .ForMember(d => d.FeeCategoryName, o => o.MapFrom(s => s.FeeCategory != null ? s.FeeCategory.Name : null));
            CreateMap<CreateStudentInvoiceItemDto, StudentInvoiceItem>();

            // Payment
            CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student != null ? s.Student.FullName : null))
                .ForMember(d => d.StudentUPI, o => o.MapFrom(s => s.Student != null ? s.Student.UPI : null))
                .ForMember(d => d.InvoiceNumber, o => o.MapFrom(s => s.StudentInvoice != null ? s.StudentInvoice.InvoiceNumber : null))
                .ForMember(d => d.BankAccountName, o => o.MapFrom(s => s.BankAccount != null ? s.BankAccount.Name : null))
                .ForMember(d => d.PaymentTypeLabel, o => o.MapFrom(s => s.PaymentType.ToString()))
                .ForMember(d => d.OriginalReceiptNumber, o => o.MapFrom(s => s.OriginalPayment != null ? s.OriginalPayment.ReceiptNumber : null))
                .ForMember(d => d.ApprovalStatus, o => o.MapFrom(s => (int)s.ApprovalStatus))
                .ForMember(d => d.ApprovalStatusLabel, o => o.MapFrom(s => s.ApprovalStatus.ToString()))
                .ForMember(d => d.Allocations, o => o.MapFrom(s => s.Allocations));
            CreateMap<CreatePaymentDto, Payment>();

            CreateMap<PaymentAllocation, PaymentAllocationDto>()
                .ForMember(d => d.FeeCategoryName, o => o.MapFrom(s =>
                    s.StudentInvoiceItem != null && s.StudentInvoiceItem.FeeCategory != null
                    ? s.StudentInvoiceItem.FeeCategory.Name : null))
                .ForMember(d => d.ItemAmount, o => o.MapFrom(s =>
                    s.StudentInvoiceItem != null ? s.StudentInvoiceItem.Amount : 0));
            CreateMap<PaymentDto, Payment>();

            // Expense Category
            CreateMap<ExpenseCategory, ExpenseCategoryDto>()
                .ForMember(d => d.ExpenseAccountName, o => o.MapFrom(s => s.ExpenseAccount != null ? s.ExpenseAccount.Name : null));
            CreateMap<CreateExpenseCategoryDto, ExpenseCategory>();
            CreateMap<ExpenseCategoryDto, ExpenseCategory>();

            // Expense
            CreateMap<Expense, ExpenseDto>()
                .ForMember(d => d.PaidFromAccountName, o => o.MapFrom(s => s.PaidFromAccount != null ? s.PaidFromAccount.Name : null))
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
                .ForMember(d => d.StatusLabel, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.Lines.Sum(l => l.Amount)))
                .ForMember(d => d.LineCount, o => o.MapFrom(s => s.Lines.Count))
                .ForMember(d => d.Lines, o => o.MapFrom(s => s.Lines));
            CreateMap<CreateExpenseDto, Expense>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (ExpenseStatus)s.Status));

            CreateMap<ExpenseLine, ExpenseLineDto>()
                .ForMember(d => d.ExpenseCategoryName, o => o.MapFrom(s => s.ExpenseCategory != null ? s.ExpenseCategory.Name : null))
                .ForMember(d => d.BudgetName, o => o.MapFrom(s => s.BudgetLine != null && s.BudgetLine.Budget != null ? s.BudgetLine.Budget.Name : null))
                .ForMember(d => d.BudgetLineAccountName, o => o.MapFrom(s => s.BudgetLine != null && s.BudgetLine.Account != null ? s.BudgetLine.Account.Name : null));
            CreateMap<CreateExpenseLineDto, ExpenseLine>();

            // Journal Entry
            CreateMap<JournalEntry, JournalEntryDto>()
                .ForMember(d => d.TotalDebit, o => o.MapFrom(s => s.Lines.Sum(l => l.Debit)))
                .ForMember(d => d.TotalCredit, o => o.MapFrom(s => s.Lines.Sum(l => l.Credit)))
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
                .ForMember(d => d.StatusLabel, o => o.MapFrom(s => s.Status.ToString()));
            CreateMap<CreateJournalEntryDto, JournalEntry>();

            CreateMap<JournalLine, JournalLineDto>()
                .ForMember(d => d.AccountCode, o => o.MapFrom(s => s.Account != null ? s.Account.Code : null))
                .ForMember(d => d.AccountName, o => o.MapFrom(s => s.Account != null ? s.Account.Name : null));
            CreateMap<CreateJournalLineDto, JournalLine>();

            // Budget
            CreateMap<Budget, BudgetDto>()
                .ForMember(d => d.AcademicYearName, o => o.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
                .ForMember(d => d.DepartmentName, o => o.MapFrom(s => s.Department != null ? s.Department.Name : null))
                .ForMember(d => d.BudgetMasterName, o => o.MapFrom(s => s.BudgetMaster != null ? s.BudgetMaster.Name : null))
                .ForMember(d => d.TotalBudgeted, o => o.MapFrom(s => s.Lines.Sum(l => l.BudgetedAmount)))
                .ForMember(d => d.Lines, o => o.MapFrom(s => s.Lines));
            CreateMap<CreateBudgetDto, Budget>();

            // Budget Master
            CreateMap<BudgetMaster, BudgetMasterDto>()
                .ForMember(d => d.AcademicYearName, o => o.MapFrom(s => s.AcademicYear != null ? s.AcademicYear.Name : null))
                .ForMember(d => d.BudgetCount, o => o.MapFrom(s => s.Budgets.Count));
            CreateMap<CreateBudgetMasterDto, BudgetMaster>();

            // Budget Amendment
            CreateMap<BudgetAmendment, BudgetAmendmentDto>()
                .ForMember(d => d.BudgetName, o => o.MapFrom(s => s.Budget != null ? s.Budget.Name : null))
                .ForMember(d => d.ApprovedByName, o => o.MapFrom(s => s.ApprovedBy != null ? s.ApprovedBy.UserName : null));
            CreateMap<CreateBudgetAmendmentDto, BudgetAmendment>();

            CreateMap<BudgetAmendmentLine, BudgetAmendmentLineDto>()
                .ForMember(d => d.AccountCode, o => o.MapFrom(s => s.Account != null ? s.Account.Code : null))
                .ForMember(d => d.AccountName, o => o.MapFrom(s => s.Account != null ? s.Account.Name : null));
            CreateMap<CreateBudgetAmendmentLineDto, BudgetAmendmentLine>();

            CreateMap<BudgetLine, BudgetLineDto>()
                .ForMember(d => d.AccountCode, o => o.MapFrom(s => s.Account != null ? s.Account.Code : null))
                .ForMember(d => d.AccountName, o => o.MapFrom(s => s.Account != null ? s.Account.Name : null))
                .ForMember(d => d.AccountType, o => o.MapFrom(s => s.Account != null ? s.Account.AccountType.ToString() : null));
            CreateMap<CreateBudgetLineDto, BudgetLine>();
        }
    }
}
