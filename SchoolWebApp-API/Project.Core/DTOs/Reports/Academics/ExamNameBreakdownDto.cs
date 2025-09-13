using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class ExamNameBreakdownDto
    {
        public int ExamNameId { get; set; }
        public double? RawScore { get; set; }
        public double ExamMark { get; set; }
        public double ContributingMark { get; set; }
        public double? WeightedScore { get; set; }
        public double? PercentScore { get; set; }
    }
}
