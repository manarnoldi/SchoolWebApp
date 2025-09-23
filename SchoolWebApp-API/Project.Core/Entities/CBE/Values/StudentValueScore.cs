using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.CBE.Values
{
    public class StudentValueScore: Base
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; } = null;
        public int ValueId { get; set; }
        public Value? Value { get; set; } = null;
        public int SessionId { get; set; }
        public Session? Session { get; set; } = null;
        public int ValueScoreId { get; set; }
        public ValueScore? ValueScore { get; set; }
        public string? Description { get; set; } = null;
    }
}
