using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Project.DataMigration;

// ApplicationDbContext.SaveChangesAsync reads HttpContext.User."username" to stamp
// CreatedBy/ModifiedBy. In a console app there is no real HttpContext, so we
// build one in-memory holding a ClaimsPrincipal with the desired username claim.
public sealed class MigrationHttpContextAccessor : IHttpContextAccessor
{
    public MigrationHttpContextAccessor(string username)
    {
        var identity = new ClaimsIdentity(new[] { new Claim("username", username) }, "migration");
        HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };
    }

    public HttpContext? HttpContext { get; set; }
}
