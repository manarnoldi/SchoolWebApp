using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs;
using SchoolWebApp.Core.DTOs.Reports.Staff;
using SchoolWebApp.Core.DTOs.Staff.StaffAttendance;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;

namespace SchoolWebApp.Infrastructure.Repositories.Staff
{
    public class StaffAttendanceRepository : BaseRepository<StaffAttendance>, IStaffAttendanceRepository
    {
        private readonly IMapper _mapper;
        public StaffAttendanceRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
        }

        public async Task<List<StaffAttendance>> GetByStaffDetailsId(int staffDetailsId)
        {
            var staffAttendances = await _dbContext.StaffAttendances
                .Where(e => e.StaffDetailsId == staffDetailsId)
                .Include(s => s.StaffDetails)
                .OrderBy(s => s.Date)
                .ToListAsync();
            return staffAttendances;
        }

        public async Task<StaffAttendance> GetByStaffAttendanceDate(int staffId, DateOnly attendanceDate)
        {
            var staffAttendance = await _dbContext.StaffAttendances
                .Include(s => s.StaffDetails)
                .Where(s => s.StaffDetailsId == staffId && DateOnly.FromDateTime(s.Date) == attendanceDate)
                .FirstOrDefaultAsync();

            return staffAttendance;
        }

        public async Task<List<StaffAttendance>> GetByMonthYearStaffId(int month, int year, int staffId)
        {
            // Get the first and last day of the month
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1); // Last day of the month

            // Generate a list of all dates in the given month
            var allDatesInMonth = Enumerable.Range(0, (endDate - startDate).Days + 1)
                                            .Select(offset => startDate.AddDays(offset))
                                            .ToList();

            // Fetch the attendance data for the staff member in the given month and year
            var staffAttendances = await _dbContext.StaffAttendances
                .Where(a => a.StaffDetailsId == staffId && a.Date.Month == month && a.Date.Year == year)
                .Include(s => s.StaffDetails)
                .ToListAsync();

            // Fetch the staff details (it will be reused for each missing attendance record)
            var staffDetails = await _dbContext.StaffDetails
                .Where(s => s.Id == staffId)
                .FirstOrDefaultAsync();

            // Initialize a list to store final results
            var result = new List<StaffAttendance>();

            // Loop through all dates in the month and ensure every day is included
            foreach (var date in allDatesInMonth)
            {
                var attendance = staffAttendances
                    .FirstOrDefault(a => a.Date.Date == date.Date);

                // If attendance record exists, add it; otherwise, add a default one (no attendance)
                if (attendance != null)
                {
                    result.Add(attendance);
                }
                else
                {
                    result.Add(new StaffAttendance
                    {
                        StaffDetailsId = staffId,
                        Date = date,
                        Present = null, // No attendance record; set to false or null
                        StaffDetails = staffDetails // Include the staff details here
                    });
                }
            }

            return result;
        }


        public async Task<List<int>> GetDistinctMonths()
        {
            var months = await _dbContext.StaffAttendances
                .Select(s => s.Date.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
            return months;
        }

        public async Task<List<int>> GetDistinctYears()
        {
            var years = await _dbContext.StaffAttendances
                .Select(s => s.Date.Year)
                .Distinct()
                .OrderByDescending(m => m)
                .ToListAsync();
            return years;
        }

        public async Task<List<StaffAttendanceReportDto>> GetStaffAttendanceReport(int month, int year, int staffCategoryId, Status status)
        {
            // Get all staff in the selected category
            var allStaffInCategory = await _dbContext.StaffDetails
                .Where(s => s.StaffCategoryId == staffCategoryId && s.Status == status)
                .ToListAsync();

            // Fetch all attendance for the specified month/year
            var staffAttends = await _dbContext
                .StaffAttendances
                .Where(a => a.Date.Month == month && a.Date.Year == year && a.StaffDetails.StaffCategoryId == staffCategoryId)
                .Include(a => a.StaffDetails)
                .ToListAsync();

            // Group attendance by staff ID
            var groupedByStaff = staffAttends
                .GroupBy(a => a.StaffDetailsId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Prepare the final report
            var staffAttendRpt = new List<StaffAttendanceReportDto>();

            foreach (var staff in allStaffInCategory)
            {
                var attendanceDto = new StaffAttendanceReportDto
                {
                    StaffDetailId = staff.Id,
                    StaffDetail = _mapper.Map<StaffDetailDto>(staff),
                    Month = month,
                    Year = year
                };

                if (groupedByStaff.TryGetValue(staff.Id, out var attendanceRecords))
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

                staffAttendRpt.Add(attendanceDto);
            }

            return staffAttendRpt;


        }
    }
}
