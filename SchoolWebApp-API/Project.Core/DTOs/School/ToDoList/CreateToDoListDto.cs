using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.DTOs.School.ToDoList
{
    public class CreateToDoListDto
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
