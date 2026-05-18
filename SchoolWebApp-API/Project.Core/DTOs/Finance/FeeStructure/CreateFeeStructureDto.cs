using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.FeeStructure
{
    public class CreateFeeStructureItemDto
    {
        public int? Id { get; set; }
        public int FeeCategoryId { get; set; }
        public decimal Amount { get; set; }
        public bool IsMandatory { get; set; } = true;
    }

    public class CreateFeeStructureDto
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public int AcademicYearId { get; set; }
        public int? SessionId { get; set; }
        public int? LearningLevelId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public List<CreateFeeStructureItemDto> Items { get; set; } = new();
    }
}
