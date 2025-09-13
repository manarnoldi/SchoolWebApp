using SchoolWebApp.Core.DTOs.Reports.Academics;
using SchoolWebApp.Core.DTOs.Reports.Students;
using SchoolWebApp.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Interfaces.IServices.Academics
{
    public interface IExamResultsService
    {
        Task<List<StudentPerformanceRowDto>> GetBroadSheet(int sessionId, int schoolClassId, int examNameId);
    }
}
