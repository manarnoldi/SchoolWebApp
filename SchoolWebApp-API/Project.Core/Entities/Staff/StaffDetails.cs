﻿using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Staff
{
    public class StaffDetails : Person
    {
        [Display(Name = "School identity number")]
        [StringLength(255)]
        public string? IdNumber { get; set; }

        public string? NhifNo { get; set; }
        public string? NssfNo { get; set; }
        public string? KraPinNo { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public DateTime? EndofEmploymentDate { get; set; }
        public bool CurrentlyEmployed { get; set; }

        public int StaffCategoryId { get; set; }
        public StaffCategory? StaffCategory { get; set; }
        public int DesignationId { get; set; }
        public Designation? Designation { get; set; }
        public int EmploymentTypeId { get; set; }
        public EmploymentType? EmploymentType { get; set; }

        public List<StaffSubject> StaffSubjects { get; set; } = new();
        public List<StaffAttendance> StaffAttendances { get; set; } = new();
        public List<StaffDiscipline> StaffDisciplines { get; set; } = new();
        public List<Department> Departments { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();
        public List<ToDoList> ToDoLists { get; set; } = new();
    }
}
