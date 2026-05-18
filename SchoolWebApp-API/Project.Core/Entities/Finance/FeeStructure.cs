using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class FeeStructure : Base
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public int? SessionId { get; set; }
        public Session? Session { get; set; }

        public int? LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<FeeStructureItem> Items { get; set; } = new();
    }
}
