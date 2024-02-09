using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Staff
{
    public class StaffClass: Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails StaffDetails { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass SchoolClass { get; set; }

        [StringLength(500)]
        public string Remarks { get; set; }        
    }
}
