namespace SchoolWebApp.Core.DTOs.Finance.FeeStructure
{
    public class FeeStructureItemDto : CreateFeeStructureItemDto
    {
        public string? FeeCategoryName { get; set; }
    }

    public class FeeStructureDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }
        public int? SessionId { get; set; }
        public string? SessionName { get; set; }
        public int? LearningLevelId { get; set; }
        public string? LearningLevelName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalAmount { get; set; }
        public List<FeeStructureItemDto> Items { get; set; } = new();
    }
}
