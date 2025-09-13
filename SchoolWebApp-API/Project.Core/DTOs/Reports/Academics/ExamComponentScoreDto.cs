using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class ExamComponentScoreDto
    {
        public string ExamName { get; set; }        // e.g., "Mid Term"
        public float? Score { get; set; }           // Raw score (null if missed)
        public float ExamMark { get; set; }         // Max mark of the exam
        public float Weight { get; set; }           // Weight toward subject total
        public float? WeightedContribution { get; set; }  // What this exam added to the subject score
    }
}
