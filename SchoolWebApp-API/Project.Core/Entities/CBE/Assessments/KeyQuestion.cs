namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class KeyQuestion: Shared.SettingsBase
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; }        
    }
}
