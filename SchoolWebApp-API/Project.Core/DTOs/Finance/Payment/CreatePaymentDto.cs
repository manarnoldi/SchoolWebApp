using SchoolWebApp.Core.Entities.Finance;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.Payment
{
    public class PaymentItemAllocationDto
    {
        public int InvoiceItemId { get; set; }
        public decimal Amount { get; set; }
    }

    public class CreatePaymentDto
    {
        [StringLength(50)]
        public string? ReceiptNumber { get; set; }

        public PaymentType PaymentType { get; set; } = PaymentType.Receipt;

        public int StudentId { get; set; }
        public int? StudentInvoiceId { get; set; }
        public int? OriginalPaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? BankAccountId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public List<PaymentItemAllocationDto>? ItemAllocations { get; set; }
    }
}
