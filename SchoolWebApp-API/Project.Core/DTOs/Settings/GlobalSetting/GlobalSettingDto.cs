namespace SchoolWebApp.Core.DTOs.Settings.GlobalSetting
{
    public class GlobalSettingDto
    {
        public int Id { get; set; }
        public string Module { get; set; }
        public string SettingKey { get; set; }
        public string SettingValue { get; set; }
        public string? Description { get; set; }
    }
}
