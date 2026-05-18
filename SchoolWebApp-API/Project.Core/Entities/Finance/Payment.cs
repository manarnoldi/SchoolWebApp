using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum PaymentMethod
    {
        Cash = 0,
        Mpesa = 1,
        BankTransfer = 2,
        Cheque = 3,
        CardPayment = 4,
        Other = 5
    }

    public enum PaymentType
    {
        Receipt = 0,
        CreditNote = 1,
        DebitNote = 2
    }

    public enum PaymentApprovalStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Rejected = 3
    }

    public class Payment : Base
    {
        [Required]
        [StringLength(50)]
        public required string ReceiptNumber { get; set; }

        public PaymentType PaymentType { get; set; } = PaymentType.Receipt;

        public PaymentApprovalStatus ApprovalStatus { get; set; } = PaymentApprovalStatus.Approved;

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int? StudentInvoiceId { get; set; }
        public StudentInvoice? StudentInvoice { get; set; }

        public int? OriginalPaymentId { get; set; }
        public Payment? OriginalPayment { get; set; }

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? BankAccountId { get; set; }
        public Account? BankAccount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public List<PaymentAllocation> Allocations { get; set; } = new();
    }

    public class PaymentAllocation : Base
    {
        public int PaymentId { get; set; }
        public Payment? Payment { get; set; }

        public int StudentInvoiceItemId { get; set; }
        public StudentInvoiceItem? StudentInvoiceItem { get; set; }

        public decimal Amount { get; set; }
    }
}
