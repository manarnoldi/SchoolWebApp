using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.School
{
    public class SchoolStream: Base
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<SchoolClass> SchoolClasses { get; set; }
    }
}
