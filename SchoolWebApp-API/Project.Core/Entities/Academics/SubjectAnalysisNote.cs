using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.Academics
{
    /// <summary>
    /// The teacher-authored "Conclusion &amp; Action Plan" saved against the
    /// Subject/Class Performance Trend report for a (year, class, subject,
    /// school exam). One row per combination, so the plan can be refreshed after
    /// each exam. SubjectId 0 = a class-level (all-subjects) note. The tables,
    /// charts and factual narrative are recomputed on every load; only this note
    /// is persisted so it is remembered and reprints.
    /// </summary>
    public class SubjectAnalysisNote : Base
    {
        public int AcademicYearId { get; set; }
        public int SchoolClassId { get; set; }
        public int SubjectId { get; set; }
        // The exam (which also fixes the term) the action plan is written for.
        public int SchoolExamId { get; set; }

        [Column(TypeName = "longtext")]
        public string? ActionPlan { get; set; }
    }
}
