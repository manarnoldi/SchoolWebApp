using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolWebApp.Core.DTOs.Reports.Students;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Interfaces.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Interfaces.IServices
{
    public interface IStudentAttendanceService
    {
        Task<List<StudentAttendanceReportDto>> GetStudentAttendanceReport(int month, int schoolClassId, Status status);
    }
}
