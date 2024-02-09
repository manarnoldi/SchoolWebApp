﻿using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Staff
{
    public class StaffSubject : Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails StaffDetails { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int AcademicYearId { get; set; }
        public AcademicYear AcademicYear { get; set; }
    }
}
