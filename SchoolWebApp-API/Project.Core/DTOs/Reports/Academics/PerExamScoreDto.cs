using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class PerExamScoreDto
    {
        public string ExamName { get; set; }    // e.g., "End Term"
        public float TotalScore { get; set; }
        public int Rank { get; set; }
    }
}
