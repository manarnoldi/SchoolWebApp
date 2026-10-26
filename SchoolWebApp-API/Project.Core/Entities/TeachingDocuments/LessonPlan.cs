using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.TeachingDocuments
{
    /// <summary>
    /// A week's lesson plans for one scheme (teacher x subject x class x term),
    /// submitted and approved together (approval entity type "LessonPlanWeek").
    /// </summary>
    public class LessonPlanWeek : Base
    {
        public int SchemeOfWorkId { get; set; }
        public SchemeOfWork? SchemeOfWork { get; set; }
        public int WeekNo { get; set; }
        public List<LessonPlan> Plans { get; set; } = new();
    }

    /// <summary>
    /// One lesson's plan. Prepared from its scheme lesson (strand, sub-strand,
    /// outcomes, key question and resources are copied in) and completed by the
    /// teacher. Summary and Reflection are written after the lesson, so they stay
    /// editable once the week is approved.
    /// </summary>
    public class LessonPlan : Base
    {
        public int LessonPlanWeekId { get; set; }
        public LessonPlanWeek? LessonPlanWeek { get; set; }
        public int? SchemeOfWorkLessonId { get; set; }
        public SchemeOfWorkLesson? SchemeOfWorkLesson { get; set; }
        public int LessonNo { get; set; }
        public DateTime? LessonDate { get; set; }
        // "HH:mm"
        [StringLength(5)]
        public string? StartTime { get; set; }
        [StringLength(5)]
        public string? EndTime { get; set; }
        public int? Roll { get; set; }
        public int? StrandId { get; set; }
        public Strand? Strand { get; set; }
        public int? SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
        [StringLength(255)]
        public string? StrandName { get; set; }
        [StringLength(255)]
        public string? SubStrandName { get; set; }
        public string? LearningOutcomes { get; set; }
        public string? KeyInquiryQuestions { get; set; }
        public string? LearningResources { get; set; }
        public string? OrganisationOfLearning { get; set; }
        public string? Introduction { get; set; }
        public int? IntroductionMinutes { get; set; }
        public int? DevelopmentMinutes { get; set; }
        public string? Conclusion { get; set; }
        public int? ConclusionMinutes { get; set; }
        public string? ExtendedActivities { get; set; }
        public string? Summary { get; set; }
        public string? Reflection { get; set; }
        public List<LessonPlanStep> Steps { get; set; } = new();
    }

    /// <summary>A numbered step of the lesson development.</summary>
    public class LessonPlanStep : Base
    {
        public int LessonPlanId { get; set; }
        public LessonPlan? LessonPlan { get; set; }
        public int StepNo { get; set; }
        public string? Activity { get; set; }
    }
}
