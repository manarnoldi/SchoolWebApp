using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs.Reports.Staff;
using SchoolWebApp.Core.DTOs.Reports.Students;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using System;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentAttendanceRepository : BaseRepository<StudentAttendance>, IStudentAttendanceRepository
    {
        private readonly IMapper _mapper;
        public StudentAttendanceRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<List<StudentAttendance>> GetByStudentClassId(int studentClassId)
        {
            var studentAttendances = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(e => e.StudentClassId == studentClassId).ToListAsync();
            return studentAttendances;
        }

        public async Task<List<StudentAttendance>> GetByStudentId(int studentId)
        {
            var studentAttendances = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(e => e.StudentClass.StudentId == studentId).ToListAsync();
            return studentAttendances;
        }

        public async Task<StudentAttendance> GetByStudentClassAttendanceDate(int studentClassId, DateOnly attendanceDate)
        {
            var studentAttendance = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(s => s.StudentClassId == studentClassId && DateOnly.FromDateTime(s.Date) == attendanceDate)
                .FirstOrDefaultAsync();

            return studentAttendance;
        }

        public async Task<List<int>> GetDistinctMonths()
        {
            var months = await _dbContext.StudentAttendances
                .Select(s => s.Date.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
            return months;
        }

        public async Task<List<int>> GetDistinctYears()
        {
            var years = await _dbContext.StudentAttendances
                .Select(s => s.Date.Year)
                .Distinct()
                .OrderByDescending(m => m)
                .ToListAsync();
            return years;
        }

        public async Task<List<StudentAttendance>> GetByMonthStudentClassId(int month, int studentClassId)
        {
            var studentClass = await _dbContext.StudentClasses
                .Where(s =>s.Id == studentClassId)
                .Include(s => s.SchoolClass)
                .ThenInclude(s => s.AcademicYear)
                .FirstOrDefaultAsync();

            // Get the first and last day of the month
            var startDate = new DateTime(studentClass.SchoolClass.AcademicYear.StartDate.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1); // Last day of the month

            // Generate a list of all dates in the given month
            var allDatesInMonth = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                            .Select(offset => startDate.AddDays(offset))
                                            .ToList();
            

            // Fetch the attendance data for the student member in the given month and year
            var studentAttendances = await _dbContext.StudentAttendances
             .Where(a => a.StudentClassId == studentClassId && a.Date.Month == month)
              .Include(s => s.StudentClass)
             .ToListAsync();

            // Fetch the student details (it will be reused for each missing attendance record)
            var studentDetails = await _dbContext.Students
                .Where(s => s.Id == studentClass.StudentId)
                .FirstOrDefaultAsync();

            // Initialize a list to store final results
            var result = new List<StudentAttendance>();

            // Loop through all dates in the month and ensure every day is included
            foreach (var date in allDatesInMonth)
            {
                var attendance = studentAttendances
                    .FirstOrDefault(a => a.Date.Date == date.Date);

                // If attendance record exists, add it; otherwise, add a default one (no attendance)
                if (attendance != null)
                {
                    result.Add(attendance);
                }
                else
                {
                    result.Add(new StudentAttendance
                    {
                        StudentClassId = studentClassId,
                        Date = date,
                        Present = null, // No attendance record; set to false or null
                        StudentClass = studentClass // Include the staff details here
                    });
                }
            }

            return result;
        }

        public async Task<List<StudentAttendanceReportDto>> GetStudentAttendanceReport(int month, int schoolClassId, Status status)
        {
            var schoolClass = await _dbContext.SchoolClasses.FindAsync(schoolClassId);
            // Get all students in the selected school class
            var students = await _dbContext.StudentClasses
                .Where(s => s.SchoolClassId == schoolClassId && s.Student.Status == status)
                .Select(sc => sc.Student)
                .ToListAsync();

            // Fetch all attendance for the specified month/year
            var studentAttends = await _dbContext
                .StudentAttendances
                .Where(a => a.Date.Month == month && a.StudentClass.SchoolClassId == schoolClassId)
                .Include(a => a.StudentClass.Student)
                .ToListAsync();

            // Group attendance by Student ID
            var groupedByStudent = studentAttends
                .GroupBy(a => a.StudentClass.Student.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Prepare the final report
            var studentAttendRpt = new List<StudentAttendanceReportDto>();

            foreach (var student in students)
            {
                var attendanceDto = new StudentAttendanceReportDto
                {
                    StudentId = student.Id,
                    Student = _mapper.Map<StudentDto>(student),
                    Month = month,
                    //Year = int.Parse(schoolClass.AcademicYear.Name)
                    Year = 2025
                };

                if (groupedByStudent.TryGetValue(student.Id, out var attendanceRecords))
                {
                    foreach (var record in attendanceRecords)
                    {
                        var day = record.Date.Day;
                        var value = (bool)record.Present ? "P" : "A";

                        switch (day)
                        {
                            case 1: attendanceDto.Day1 = value; break;
                            case 2: attendanceDto.Day2 = value; break;
                            case 3: attendanceDto.Day3 = value; break;
                            case 4: attendanceDto.Day4 = value; break;
                            case 5: attendanceDto.Day5 = value; break;
                            case 6: attendanceDto.Day6 = value; break;
                            case 7: attendanceDto.Day7 = value; break;
                            case 8: attendanceDto.Day8 = value; break;
                            case 9: attendanceDto.Day9 = value; break;
                            case 10: attendanceDto.Day10 = value; break;
                            case 11: attendanceDto.Day11 = value; break;
                            case 12: attendanceDto.Day12 = value; break;
                            case 13: attendanceDto.Day13 = value; break;
                            case 14: attendanceDto.Day14 = value; break;
                            case 15: attendanceDto.Day15 = value; break;
                            case 16: attendanceDto.Day16 = value; break;
                            case 17: attendanceDto.Day17 = value; break;
                            case 18: attendanceDto.Day18 = value; break;
                            case 19: attendanceDto.Day19 = value; break;
                            case 20: attendanceDto.Day20 = value; break;
                            case 21: attendanceDto.Day21 = value; break;
                            case 22: attendanceDto.Day22 = value; break;
                            case 23: attendanceDto.Day23 = value; break;
                            case 24: attendanceDto.Day24 = value; break;
                            case 25: attendanceDto.Day25 = value; break;
                            case 26: attendanceDto.Day26 = value; break;
                            case 27: attendanceDto.Day27 = value; break;
                            case 28: attendanceDto.Day28 = value; break;
                            case 29: attendanceDto.Day29 = value; break;
                            case 30: attendanceDto.Day30 = value; break;
                            case 31: attendanceDto.Day31 = value; break;
                        }
                    }
                }

                studentAttendRpt.Add(attendanceDto);
            }

            return studentAttendRpt;
        }

    }
}
