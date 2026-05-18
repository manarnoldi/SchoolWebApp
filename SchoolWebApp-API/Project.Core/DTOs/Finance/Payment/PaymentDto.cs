using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.Payment
{
    public class NoteRequestDto
    {
        public decimal Amount { get; set; }

        [Required, StringLength(500)]
        public required string Reason { get; set; }
    }

    public class PaymentAllocationDto
    {
        public int Id { get; set; }
        public int StudentInvoiceItemId { get; set; }
        public string? FeeCategoryName { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentDto : CreatePaymentDto
    {
        public int Id { get; set; }
        public string? StudentName { get; set; }
        public string? StudentUPI { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? BankAccountName { get; set; }
        public string? PaymentTypeLabel { get; set; }
        public string? OriginalReceiptNumber { get; set; }
        public int ApprovalStatus { get; set; }
        public string? ApprovalStatusLabel { get; set; }
        public List<PaymentAllocationDto> Allocations { get; set; } = new();
    }
}
