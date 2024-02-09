using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Shared
{
    public class Base
    {
        public int Id { get; private set; }

        [ScaffoldColumn(false)]
        public DateTime Created { get; set; }

        [StringLength(255)]
        [ScaffoldColumn(false)]
        public string CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime Modified { get; set; }

        [StringLength(255)]
        [ScaffoldColumn(false)]
        public string ModifiedBy { get; set; }
    }
}
