using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.TeachingDocuments
{
    /// <summary>
    /// A teacher's scheme of work for one subject, one class and one term. It is
    /// approved once per term (approval entity type "SchemeOfWork"); its lessons
    /// are what lesson plans are prepared from and what the record of work is
    /// kept against.
    /// </summary>
    public class SchemeOfWork : Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public int SessionId { get; set; }
        public Session? Session { get; set; }

        // Books and guides the scheme follows, e.g. "KLB Visionary SST Grade 6 TG and Learner's Book".
        [StringLength(500)]
        public string? References { get; set; }
        public int NumberOfWeeks { get; set; }
        public int LessonsPerWeek { get; set; }
        public List<SchemeOfWorkLesson> Lessons { get; set; } = new();
    }

    /// <summary>
    /// One row of the scheme: a lesson in a given week. The strand and sub-strand
    /// are picked from the curriculum (which is how sub-strands get mapped to
    /// terms) but the text is the teacher's own copy, so editing the curriculum
    /// later never rewrites a submitted scheme.
    /// </summary>
    public class SchemeOfWorkLesson : Base
    {
        public int SchemeOfWorkId { get; set; }
        public SchemeOfWork? SchemeOfWork { get; set; }
        public int WeekNo { get; set; }
        public int LessonNo { get; set; }
        public int? StrandId { get; set; }
        public Strand? Strand { get; set; }
        public int? SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
        [StringLength(255)]
        public string? StrandName { get; set; }
        [StringLength(255)]
        public string? SubStrandName { get; set; }
        public string? LearningOutcomes { get; set; }
        public string? LearningExperiences { get; set; }
        public string? KeyInquiryQuestions { get; set; }
        public string? LearningResources { get; set; }
        public string? AssessmentMethods { get; set; }
        public string? Reflection { get; set; }
        public RecordOfWorkEntry? RecordOfWorkEntry { get; set; }
    }
}
