using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Students
{
    public class Parent : Person
    {
        [Display(Name = "Notify about student progress")]
        public bool Notifiable { get; set; }

        [Display(Name = "Pays school fees")]
        public bool Payer { get; set; }

        [Display(Name = "Picks up student")]
        public bool Pickup { get; set; }      
        
        public int OccupationId { get; set; }
        public Occupation? Occupation { get; set; }

        public List<Student> Students { get; set; } = new();
        public List<StudentParent> StudentParents { get; set; } = new();

    }
}
