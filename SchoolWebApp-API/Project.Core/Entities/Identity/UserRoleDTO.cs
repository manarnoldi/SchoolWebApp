using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolSoftWebApi.Models.Identity
{
    public class UserRoleDTO
    {
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}