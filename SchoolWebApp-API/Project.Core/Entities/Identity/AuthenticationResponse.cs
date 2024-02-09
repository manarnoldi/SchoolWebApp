using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class AuthenticationResponse
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}