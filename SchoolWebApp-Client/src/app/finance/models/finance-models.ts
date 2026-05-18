import {ResourceModel} from '@/core/models/ResourceModel';

export enum InvoiceStatus {
    Unpaid = 0,
    PartiallyPaid = 1,
    Paid = 2,
    Overdue = 3,
    Cancelled = 4
}

export enum PaymentMethod {
    Cash = 0,
    Mpesa = 1,
    BankTransfer = 2,
    Cheque = 3,
    CardPayment = 4,
    Other = 5
}

export class FeeCategory extends ResourceModel<FeeCategory> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public incomeAccountId?: number;
    public incomeAccountName?: string;
    public isActive?: boolean;
    constructor(m?: Partial<FeeCategory>) { super(m); }
}

export class FeeStructureItem {
    public id?: number;
    public feeCategoryId?: number;
    public feeCategoryName?: string;
    public amount?: number;
    public isMandatory?: boolean;
}

export class FeeStructure extends ResourceModel<FeeStructure> {
    public name?: string;
    public academicYearId?: number;
    public academicYearName?: string;
    public sessionId?: number;
    public sessionName?: string;
    public learningLevelId?: number;
    public learningLevelName?: string;
    public description?: string;
    public isActive?: boolean;
    public totalAmount?: number;
    public items!: FeeStructureItem[];
    constructor(m?: Partial<FeeStructure>) { super(m); if (!this.items) this.items = []; }
}

export class StudentInvoiceItem {
    public id?: number;
    public feeCategoryId?: number;
    public feeCategoryName?: string;
    public amount?: number;
    public discount?: number;
    public paidAmount?: number;
    public description?: string;
}

export class StudentInvoice extends ResourceModel<StudentInvoice> {
    public invoiceNumber?: string;
    public studentId?: number;
    public studentName?: string;
    public studentUPI?: string;
    public academicYearId?: number;
    public academicYearName?: string;
    public sessionId?: number;
    public sessionName?: string;
    public invoiceDate?: Date | string;
    public dueDate?: Date | string;
    public totalAmount?: number;
    public paidAmount?: number;
    public discountAmount?: number;
    public balance?: number;
    public status?: InvoiceStatus;
    public description?: string;
    public items!: StudentInvoiceItem[];
    constructor(m?: Partial<StudentInvoice>) { super(m); if (!this.items) this.items = []; }
}

export enum PaymentType { Receipt = 0, CreditNote = 1, DebitNote = 2 }

export class PaymentAllocation {
    public id?: number;
    public studentInvoiceItemId?: number;
    public feeCategoryName?: string;
    public itemAmount?: number;
    public amount?: number;
}

export class Payment extends ResourceModel<Payment> {
    public receiptNumber?: string;
    public paymentType?: PaymentType;
    public studentId?: number;
    public studentName?: string;
    public studentUPI?: string;
    public studentInvoiceId?: number | null;
    public invoiceNumber?: string;
    public originalPaymentId?: number;
    public originalReceiptNumber?: string;
    public paymentDate?: Date | string;
    public amount?: number;
    public paymentMethod?: PaymentMethod;
    public transactionReference?: string;
    public bankAccountId?: number | null;
    public bankAccountName?: string;
    public description?: string;
    public reason?: string;
    public paymentTypeLabel?: string;
    public approvalStatus?: number;
    public approvalStatusLabel?: string;
    public allocations!: PaymentAllocation[];
    constructor(m?: Partial<Payment>) { super(m); if (!this.allocations) this.allocations = []; }
}

export class ExpenseCategory extends ResourceModel<ExpenseCategory> {
    public name?: string;
    public description?: string;
    public rank?: number;
    public expenseAccountId?: number;
    public expenseAccountName?: string;
    public isActive?: boolean;
    constructor(m?: Partial<ExpenseCategory>) { super(m); }
}

export class ExpenseLine {
    public id?: number;
    public expenseCategoryId?: number;
    public expenseCategoryName?: string;
    public amount?: number;
    public vendor?: string;
    public budgetLineId?: number;
    public budgetName?: string;
    public budgetLineAccountName?: string;
    public description?: string;
}

export class Expense extends ResourceModel<Expense> {
    public referenceNumber?: string;
    public expenseDate?: Date | string;
    public paymentMethod?: PaymentMethod;
    public transactionReference?: string;
    public paidFromAccountId?: number;
    public paidFromAccountName?: string;
    public status?: number;
    public statusLabel?: string;
    public totalAmount?: number;
    public lineCount?: number;
    public description?: string;
    public lines!: ExpenseLine[];
    constructor(m?: Partial<Expense>) { super(m); if (!this.lines) this.lines = []; }
}

export class JournalLine {
    public id?: number;
    public accountId?: number;
    public accountCode?: string;
    public accountName?: string;
    public debit?: number;
    public credit?: number;
    public description?: string;
}

export class JournalEntry extends ResourceModel<JournalEntry> {
    public referenceNumber?: string;
    public entryDate?: Date | string;
    public description?: string;
    public isPosted?: boolean;
    public status?: number;
    public statusLabel?: string;
    public totalDebit?: number;
    public totalCredit?: number;
    public lines!: JournalLine[];
    constructor(m?: Partial<JournalEntry>) { super(m); if (!this.lines) this.lines = []; }
}
