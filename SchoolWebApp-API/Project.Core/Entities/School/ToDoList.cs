using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.School
{
    public class ToDoList: Base
    {
        public required string ItemName { get; set; }
        public DateTime CompleteBy { get; set; }
        public Boolean Completed { get; set; }

        public int? UserId { get; set; }

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
