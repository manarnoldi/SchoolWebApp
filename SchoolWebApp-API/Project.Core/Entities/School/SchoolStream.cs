using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.School
{
    public class SchoolStream: Base
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Abbreviation { get; set; }
        public int Rank { get; set; }
        public List<SchoolClass> SchoolClasses { get; set; } = new();
    }
}
