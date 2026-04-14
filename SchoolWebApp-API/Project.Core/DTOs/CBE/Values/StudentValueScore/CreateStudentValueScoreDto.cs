using SchoolWebApp.Core.DTOs.CBE.Values.Value;
using SchoolWebApp.Core.DTOs.CBE.Values.ValueScore;

namespace SchoolWebApp.Core.DTOs.CBE.Values.StudentValueScore
{
    public class CreateStudentValueScoreDto
    {
        public int StudentId { get; set; }
        public int ValueId { get; set; }
        public ValueDto? Value { get; set; }
        public int SessionId { get; set; }
        public int ValueScoreId { get; set; }
        public ValueScoreDto? ValueScore { get; set; }
        public string? Description { get; set; }
    }
}
