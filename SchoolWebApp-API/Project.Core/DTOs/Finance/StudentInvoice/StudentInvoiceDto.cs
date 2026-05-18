using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.Core.DTOs.Finance.StudentInvoice
{
    public class StudentInvoiceItemDto : CreateStudentInvoiceItemDto
    {
        public string? FeeCategoryName { get; set; }
    }

    public class StudentInvoiceDto
    {
        public int Id { get; set; }
        public string? InvoiceNumber { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentUPI { get; set; }
        public int AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }
        public int? SessionId { get; set; }
        public string? SessionName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Balance { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? Description { get; set; }
        public List<StudentInvoiceItemDto> Items { get; set; } = new();
    }
}
