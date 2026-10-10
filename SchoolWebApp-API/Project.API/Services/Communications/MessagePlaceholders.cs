using SchoolWebApp.Core.DTOs.Communications;
using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.API.Services.Communications
{
    /// <summary>The placeholders each message type fills in, for the template editor.</summary>
    public static class MessagePlaceholders
    {
        private static PlaceholderDto P(string key, string description) => new() { Key = "{" + key + "}", Description = description };

        private static readonly List<PlaceholderDto> School = new()
        {
            P("SchoolName", "School name"),
            P("SchoolPhone", "School telephone"),
            P("SchoolEmail", "School email"),
            P("Date", "Today's date")
        };

        private static readonly List<PlaceholderDto> Learner = new()
        {
            P("ParentName", "Parent's name (\"Parent/Guardian\" when taken from the student record)"),
            P("StudentName", "Learner's name"),
            P("AdmissionNo", "Admission number"),
            P("ClassName", "Learner's class, e.g. Grade 4 E")
        };

        private static readonly Dictionary<string, List<PlaceholderDto>> ByType = new()
        {
            [MessageTypeCodes.Custom] = new()
            {
                P("RecipientName", "Recipient's name (parent or staff member)"),
                P("ParentName", "Parent's name - parent messages"),
                P("StudentName", "Learner name(s) - parent messages"),
                P("ClassName", "Learner's class - parent messages"),
                P("StaffName", "Staff member's name - staff messages")
            },
            [MessageTypeCodes.ExamResults] = Learner.Concat(new[]
            {
                P("ExamName", "Exam, e.g. End Term Exam"),
                P("TermName", "Term"),
                P("SubjectScores", "Each subject's score, e.g. MAT 78, ENG 65"),
                P("TotalMarks", "Total marks"),
                P("MeanScore", "Mean score"),
                P("MeanGrade", "Mean grade"),
                P("Position", "Position in class"),
                P("ClassSize", "Number of learners in the class")
            }).ToList(),
            [MessageTypeCodes.FeeInvoice] = Learner.Concat(new[]
            {
                P("InvoiceNumber", "Invoice number"),
                P("InvoiceAmount", "Invoice total"),
                P("InvoiceItems", "Invoice lines (email)"),
                P("TermName", "Term invoiced"),
                P("DueDate", "Due date"),
                P("Balance", "Learner's total fee balance")
            }).ToList(),
            [MessageTypeCodes.FeeBalance] = Learner.Concat(new[]
            {
                P("Balance", "Learner's total fee balance")
            }).ToList(),
            [MessageTypeCodes.FeePayment] = Learner.Concat(new[]
            {
                P("AmountPaid", "Amount received"),
                P("ReceiptNumber", "Receipt number"),
                P("Balance", "Fee balance after the payment")
            }).ToList()
        };

        public static List<PlaceholderDto> For(string? code)
        {
            var specific = ByType.GetValueOrDefault(code ?? MessageTypeCodes.Custom) ?? ByType[MessageTypeCodes.Custom];
            return specific.Concat(School).ToList();
        }
    }
}
