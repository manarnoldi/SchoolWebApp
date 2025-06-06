﻿using Microsoft.EntityFrameworkCore;
using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentAttendanceRepository : IBaseRepository<StudentAttendance>
    {
        Task<List<StudentAttendance>> GetByStudentClassId(int studentClassId);
        Task<List<StudentAttendance>> GetByStudentId(int studentId);
        Task<StudentAttendance> GetByStudentClassAttendanceDate(int studentClassId, DateOnly attendanceDate);
        Task<List<StudentAttendance>> GetByMonthYearStudentClassId(int month, int year, int studentClassId);
        Task<List<int>> GetDistinctMonths();
        Task<List<int>> GetDistinctYears();
    }
}
