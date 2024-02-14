using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class RoleToReturn
    {
        public string? Id { get; set; }

        public string? Name { get; set; }
        public string? NormalizedName { get; set; }
    }
}
