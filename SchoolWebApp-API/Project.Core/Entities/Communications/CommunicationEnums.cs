namespace SchoolWebApp.Core.Entities.Communications
{
    /// <summary>
    /// How a message type goes out. A template carries the school's choice; each
    /// queued message is a single channel (a "Both" send queues one SMS and one email).
    /// </summary>
    public enum MessageChannel
    {
        None = 0,
        Sms = 1,
        Email = 2,
        Both = 3
    }

    public enum MessageStatus
    {
        Queued = 0,
        Sending = 1,
        Sent = 2,
        Delivered = 3,
        Failed = 4,
        Cancelled = 5,
        Undelivered = 6
    }

    public enum RecipientType
    {
        Parent = 0,
        Staff = 1,
        // Contact details held on the student record itself - schools migrated
        // from the legacy system keep the guardian's phone there, with no
        // Parent rows linked.
        StudentContact = 2,
        Other = 3
    }

    /// <summary>
    /// Where a learner's parent contact (phone / email) is taken from - a
    /// per-school choice, since migrated schools hold it on the student record.
    /// </summary>
    public enum ParentContactSource
    {
        ParentRecord = 0,
        StudentRecord = 1,
        // The linked parents' contacts; the student record's for a learner
        // with no linked parent holding one.
        ParentThenStudent = 2
    }

    /// <summary>
    /// Codes of the standard message types. Each has a system template seeded
    /// with the module; schools edit the wording and the channel but cannot delete them.
    /// </summary>
    public static class MessageTypeCodes
    {
        public const string ExamResults = "ExamResults";
        public const string FeeInvoice = "FeeInvoice";
        public const string FeeBalance = "FeeBalance";
        public const string FeePayment = "FeePayment";
        public const string Custom = "Custom";
    }
}
