using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.DTOs.Reports.Students;
using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Services.Students
{
    public class StudentAttendanceService: IStudentAttendanceService
    {
        private readonly ILogger<IStudentAttendanceService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentAttendanceService(ILogger<StudentAttendanceService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StudentAttendanceReportDto>> GetStudentAttendanceReport(int month, int schoolClassId, Status status)
        {
            var schoolClass = await _unitOfWork.SchoolClasses.GetById(schoolClassId,includeProperties:"AcademicYear");

            var studentClasses = await _unitOfWork.StudentClasses
                .Find(s => s.SchoolClassId == schoolClassId && s.Student.Status == status,includeProperties: "Student");

            var students = studentClasses.Select(sc => sc.Student);

            // Fetch all attendance for the specified month/year
            var studentAttends = await _unitOfWork
                .StudentAttendances
                .Find(a => a.Date.Month == month && a.StudentClass.SchoolClassId == schoolClassId, includeProperties: "StudentClass.Student");

            // Group attendance by Student ID
            var groupedByStudent = studentAttends
                .GroupBy(a => a.StudentClass.Student.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Prepare the final report
            var studentAttendRpt = new List<StudentAttendanceReportDto>();

            foreach (var studentClass in studentClasses)
            {
                var attendanceDto = new StudentAttendanceReportDto
                {
                    StudentId = studentClass.Student.Id,
                    StudentClassId = studentClass.Id,
                    Student = _mapper.Map<StudentDto>(studentClass.Student),
                    Month = month,
                    Year = int.Parse(schoolClass.AcademicYear.Name)
                };

                if (groupedByStudent.TryGetValue(studentClass.Student.Id, out var attendanceRecords))
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
