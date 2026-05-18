using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum InvoiceStatus
    {
        Unpaid = 0,
        PartiallyPaid = 1,
        Paid = 2,
        Overdue = 3,
        Cancelled = 4
    }

    public class StudentInvoice : Base
    {
        [Required]
        [StringLength(50)]
        public required string InvoiceNumber { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public int? SessionId { get; set; }
        public Session? Session { get; set; }

        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DiscountAmount { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

        [StringLength(500)]
        public string? Description { get; set; }

        public List<StudentInvoiceItem> Items { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }
}
