using SchoolWebApp.Core.DTOs.Students.Parent;
using SchoolWebApp.Core.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.Students.StudentParent
{
    public class StudentParentDetailsDto: ParentDto
    {
        public int RelationShipId { get; set; }
        public string OtherDetails { get; set; }

    }
}
