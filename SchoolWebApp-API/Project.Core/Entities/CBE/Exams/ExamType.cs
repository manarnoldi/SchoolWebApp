using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Exams
{
    public class ExamType : SettingsBase
    {
        public required string Abbreviation { get; set; }

        /// <summary>
        /// Whether this type's results appear on the report form, in a column of
        /// their own. Types tracked internally only - weekly marathons, mocks - are
        /// left off. Called Internal before it was renamed for clarity.
        /// </summary>
        public bool ShowOnReportForm { get; set; }

        /// <summary>
        /// Whether a term may hold more than one exam of this type, such as a weekly
        /// marathon. Off for the usual opening, mid-term and end-term exams, so each
        /// has one exam a term and one report-form column.
        /// </summary>
        public bool AllowMultiplePerTerm { get; set; }
        public List<SchoolExam> SchoolExams { get; set; } = new ();
    }
}
