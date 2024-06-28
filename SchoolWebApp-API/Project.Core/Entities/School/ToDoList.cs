using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.School
{
    public class ToDoList: Base
    {
        public string ItemName { get; set; }
        public DateTime CompleteBy { get; set; }
        public Boolean Completed { get; set; }

        public int StaffDetailsId { get; set; }
        public StaffDetails StaffDetails { get; set; }

        [NotMapped]
        public double TimeToDeadline
        {
            get
            {
                var dateDiff = CompleteBy - DateTime.Now;
                return dateDiff.TotalMinutes;
            }
        }
    }
}
