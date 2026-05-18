using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class BudgetLine : Base
    {
        public int BudgetId { get; set; }
        public Budget? Budget { get; set; }

        public int AccountId { get; set; }
        public Account? Account { get; set; }

        public decimal BudgetedAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
