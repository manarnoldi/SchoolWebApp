using SchoolWebApp.Core.Entities.Communications;

namespace SchoolWebApp.Core.DTOs.Communications
{
    /// <summary>
    /// The gateway settings as the SuperAdministrator sees them. Secrets are never
    /// sent back: HasSmsApiKey / HasSmtpPassword say whether one is stored, and an
    /// empty value on save keeps the stored one.
    /// </summary>
    public class CommunicationSettingDto
    {
        public bool SmsEnabled { get; set; }
        public string? SmsApiUrl { get; set; }
        public string? SmsApiKey { get; set; }
        public bool HasSmsApiKey { get; set; }
        public string? SmsPartnerId { get; set; }
        public string? SmsSenderId { get; set; }
        public decimal SmsUnitPrice { get; set; }
        public bool EmailEnabled { get; set; }
        public string? SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool SmtpUseSsl { get; set; }
        public string? SmtpUsername { get; set; }
        public string? SmtpPassword { get; set; }
        public bool HasSmtpPassword { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }
        public string? ReplyToEmail { get; set; }
        public bool TestMode { get; set; }
        public string? TestPhoneNumber { get; set; }
        public string? TestEmail { get; set; }
    }

    /// <summary>What any signed-in user may know about the gateways - enough for the send screens.</summary>
    public class CommunicationStatusDto
    {
        public bool SmsEnabled { get; set; }
        public bool EmailEnabled { get; set; }
        public bool TestMode { get; set; }
        public ParentContactSource ParentContactSource { get; set; }
    }

    public class GatewayTestDto
    {
        public string? Destination { get; set; }
        public string? Message { get; set; }
    }

    public class MessageTemplateDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
        public MessageChannel Channel { get; set; }
        public bool IsActive { get; set; }
        public string? SmsBody { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }
        public DateTime? Modified { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class PlaceholderDto
    {
        public required string Key { get; set; }
        public required string Description { get; set; }
    }

    public enum RecipientGroup
    {
        AllParents = 0,
        ParentsOfClasses = 1,
        ParentsOfEducationLevels = 2,
        ParentsOfStudents = 3,
        AllStaff = 4,
        StaffByCategory = 5,
        SelectedStaff = 6,
        CustomContacts = 7
    }

    public class RecipientCriteriaDto
    {
        public RecipientGroup Group { get; set; }
        public List<int> SchoolClassIds { get; set; } = new();
        public List<int> EducationLevelIds { get; set; } = new();
        public List<int> StudentIds { get; set; } = new();
        public List<int> StaffIds { get; set; } = new();
        public List<int> StaffCategoryIds { get; set; } = new();
        /// <summary>Phone numbers and / or email addresses, one per line or comma-separated.</summary>
        public string? CustomContacts { get; set; }
    }

    public class ComposeMessageDto
    {
        public string? Title { get; set; }
        public int? MessageTemplateId { get; set; }
        public MessageChannel Channel { get; set; }
        public RecipientCriteriaDto Recipients { get; set; } = new();
        public string? SmsBody { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailBody { get; set; }
    }

    public class RecipientPreviewDto
    {
        public string? Name { get; set; }
        public RecipientType RecipientType { get; set; }
        public string? StudentNames { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class ComposePreviewDto
    {
        public int Recipients { get; set; }
        public int SmsCount { get; set; }
        public int EmailCount { get; set; }
        /// <summary>Recipients with no usable contact for the chosen channel(s).</summary>
        public int MissingContact { get; set; }
        public int SmsParts { get; set; }
        public string? SampleSms { get; set; }
        public string? SampleEmailSubject { get; set; }
        public string? SampleEmailBody { get; set; }
        public bool TestMode { get; set; }
        public List<RecipientPreviewDto> RecipientList { get; set; } = new();
    }

    public class QueueResultDto
    {
        public int BatchId { get; set; }
        public int Queued { get; set; }
        public int Skipped { get; set; }
        public bool TestMode { get; set; }
    }

    public class OutboundMessageDto
    {
        public int Id { get; set; }
        public int MessageBatchId { get; set; }
        public string? BatchTitle { get; set; }
        public required string MessageTypeCode { get; set; }
        public MessageChannel Channel { get; set; }
        public RecipientType RecipientType { get; set; }
        public string? RecipientName { get; set; }
        public required string Destination { get; set; }
        public string? OriginalDestination { get; set; }
        public string? Subject { get; set; }
        public required string Body { get; set; }
        public MessageStatus Status { get; set; }
        public int Attempts { get; set; }
        public DateTime? NextAttemptAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public string? ProviderMessageId { get; set; }
        public string? ProviderStatus { get; set; }
        public string? ErrorMessage { get; set; }
        public int SmsParts { get; set; }
        public bool IsTest { get; set; }
        public DateTime? Created { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class StatusCountDto
    {
        public MessageStatus Status { get; set; }
        public int Count { get; set; }
    }

    public class MessageQueuePageDto
    {
        public List<OutboundMessageDto> Data { get; set; } = new();
        public int TotalCount { get; set; }
        public List<StatusCountDto> StatusCounts { get; set; } = new();
    }

    public class MessageBatchDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string MessageTypeCode { get; set; }
        public MessageChannel Channel { get; set; }
        public string? RecipientSummary { get; set; }
        public int TotalMessages { get; set; }
        public bool IsTest { get; set; }
        public DateTime? Created { get; set; }
        public string? CreatedBy { get; set; }
        public int Queued { get; set; }
        public int Sent { get; set; }
        public int Delivered { get; set; }
        public int Failed { get; set; }
        public int Cancelled { get; set; }
        public int SmsParts { get; set; }
    }

    public class SmsReportRowDto
    {
        public required string Label { get; set; }
        public int Messages { get; set; }
        public int Parts { get; set; }
    }

    public class SmsReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Messages { get; set; }
        public int Parts { get; set; }
        public int Delivered { get; set; }
        public int Undelivered { get; set; }
        public int AwaitingReport { get; set; }
        public int Failed { get; set; }
        public int TestMessages { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public List<SmsReportRowDto> ByMessageType { get; set; } = new();
        public List<SmsReportRowDto> ByDay { get; set; } = new();
        public List<OutboundMessageDto> Details { get; set; } = new();
    }
}

namespace SchoolWebApp.Core.DTOs.Communications
{
    // Standard messages: each request previews (Preview = true) or queues the
    // message type's template for the learners given.

    public class SubjectScoreDto
    {
        public required string Subject { get; set; }
        public string? SubjectName { get; set; }
        public string? Score { get; set; }
        public string? Grade { get; set; }
    }

    /// <summary>
    /// One learner's results as the broadsheet computed them - with the school's
    /// grading, ranking and mean-basis settings - so the message matches the
    /// printed broadsheet.
    /// </summary>
    public class StudentResultDto
    {
        public int StudentId { get; set; }
        public List<SubjectScoreDto> Subjects { get; set; } = new();
        public string? TotalMarks { get; set; }
        public string? MeanScore { get; set; }
        public string? MeanGrade { get; set; }
        public int? Position { get; set; }
        public int? ClassSize { get; set; }
    }

    public class ExamResultsMessageDto
    {
        public bool Preview { get; set; }
        public int? SchoolExamId { get; set; }
        public required string ExamName { get; set; }
        public string? TermName { get; set; }
        public string? ClassName { get; set; }
        public List<StudentResultDto> Results { get; set; } = new();
    }

    public class InvoiceMessageDto
    {
        public bool Preview { get; set; }
        public List<int> InvoiceIds { get; set; } = new();
    }

    public class FeeBalanceMessageDto
    {
        public bool Preview { get; set; }
        public List<int> StudentIds { get; set; } = new();
        /// <summary>Only learners owing more than this are reminded.</summary>
        public decimal MinBalance { get; set; }
    }

    public class PaymentMessageDto
    {
        public bool Preview { get; set; }
        public List<int> PaymentIds { get; set; } = new();
    }
}

namespace SchoolWebApp.Core.DTOs.Communications
{
    /// <summary>
    /// A school exam's results for every class sitting it (or the classes given),
    /// worked out on the server - used when the exam is released and from the
    /// School Exams list afterwards.
    /// </summary>
    public class SchoolExamResultsMessageDto
    {
        public bool Preview { get; set; }
        public int SchoolExamId { get; set; }
        public List<int> SchoolClassIds { get; set; } = new();
    }
}
