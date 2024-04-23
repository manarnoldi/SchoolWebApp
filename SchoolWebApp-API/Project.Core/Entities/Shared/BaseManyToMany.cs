using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Entities.Shared
{
    public class BaseManyToMany
    {
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
