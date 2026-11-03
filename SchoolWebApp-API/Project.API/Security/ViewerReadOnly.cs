using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolWebApp.API.Security
{
    /// <summary>
    /// Viewer is given on top of another role: the user sees what that role sees
    /// (Administrator + Viewer sees the whole school) and changes nothing - for the
    /// demo site, or a board member or auditor at a school.
    /// </summary>
    public static class ViewerRole
    {
        public const string Name = "Viewer";
        public const string Message = "This account is view-only. Changes are not saved.";
    }

    /// <summary>A POST that changes nothing (signing in, a preview, a count), open to viewers.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowForViewerAttribute : Attribute { }

    /// <summary>
    /// Global filter: a viewer's POST, PUT, PATCH or DELETE is refused with 403 - as an authorization
    /// filter, ahead of model validation, so even a half-filled form gets this answer.
    /// </summary>
    public class ViewerReadOnlyFilter : IAuthorizationFilter
    {
        private readonly ILogger<ViewerReadOnlyFilter> _logger;

        public ViewerReadOnlyFilter(ILogger<ViewerReadOnlyFilter> logger)
        {
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var http = context.HttpContext;
            var write = !HttpMethods.IsGet(http.Request.Method) && !HttpMethods.IsHead(http.Request.Method) && !HttpMethods.IsOptions(http.Request.Method);
            if (write && http.User.IsInRole(ViewerRole.Name)
                && !context.ActionDescriptor.EndpointMetadata.OfType<AllowForViewerAttribute>().Any())
            {
                var who = http.User.FindFirst("username")?.Value ?? http.User.FindFirst("userid")?.Value ?? "?";
                _logger.LogInformation("Viewer {User} refused {Method} {Path}", who, http.Request.Method, http.Request.Path);
                // `message` and `error` both carry it: pages show one or the other.
                context.Result = new ObjectResult(new { message = ViewerRole.Message, error = ViewerRole.Message, readOnly = true })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}
