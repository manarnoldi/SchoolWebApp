using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolWebApp.API.Licensing
{
    /// <summary>
    /// Marks a controller (or action) as part of a paid module: its endpoints answer
    /// only while the school's licence includes the module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiresModuleAttribute : Attribute
    {
        public string Module { get; }

        public RequiresModuleAttribute(string module)
        {
            Module = module;
        }
    }

    /// <summary>Global filter: refuses calls to a module the licence does not include.</summary>
    public class ModuleLicenceFilter : IAsyncActionFilter
    {
        private readonly LicenceService _licence;

        public ModuleLicenceFilter(LicenceService licence)
        {
            _licence = licence;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var modules = context.ActionDescriptor.EndpointMetadata.OfType<RequiresModuleAttribute>().Select(a => a.Module).Distinct().ToList();
            if (modules.Count > 0)
            {
                var status = await _licence.StatusAsync(context.HttpContext.Request.Host.Host);
                var missing = modules.FirstOrDefault(m => !status.Modules.Contains(m));
                if (missing != null)
                {
                    context.Result = new ObjectResult($"The {missing} module is not part of this school's licence.") { StatusCode = StatusCodes.Status403Forbidden };
                    return;
                }
            }
            await next();
        }
    }
}
