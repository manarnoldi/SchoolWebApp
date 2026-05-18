using SchoolWebApp.Core.Entities.Finance;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.StudentInvoice
{
    public class CreateStudentInvoiceItemDto
    {
        public int? Id { get; set; }
        public int FeeCategoryId { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal PaidAmount { get; set; }
        public int? SponsorshipId { get; set; }
        public string? Description { get; set; }
    }

    public class CreateStudentInvoiceDto
    {
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        public int StudentId { get; set; }
        public int AcademicYearId { get; set; }
        public int? SessionId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal DiscountAmount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

        public List<CreateStudentInvoiceItemDto> Items { get; set; } = new();
    }

    public class BulkInvoiceDto
    {
        public int FeeStructureId { get; set; }
        public int SchoolClassId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
    }
}
