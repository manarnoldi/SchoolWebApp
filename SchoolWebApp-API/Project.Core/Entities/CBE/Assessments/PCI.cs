using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class PCI: SettingsBase
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }
    }
}
