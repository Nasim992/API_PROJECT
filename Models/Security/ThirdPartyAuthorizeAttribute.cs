using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dextor.API.Security // Adjust namespace as needed
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ThirdPartyAuthorizeAttribute : Attribute, IAsyncActionFilter
    {
        private const string API_KEY_HEADER_NAME = "X-Api-Key";
        private readonly string[] _allowedVendors;

        // The 'params' keyword lets you pass zero, one, or multiple vendor names
        public ThirdPartyAuthorizeAttribute(params string[] allowedVendors)
        {
            _allowedVendors = allowedVendors;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Check if the header exists
            if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { error = "API Key is missing" });
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var vendorConfigs = configuration.GetSection("ThirdPartyAuth").GetChildren();

            // 2. Find which Vendor matches the provided token
            var matchedVendor = vendorConfigs.FirstOrDefault(v => v.Value == extractedApiKey.ToString());

            // If no vendor has this key, reject them
            if (matchedVendor == null)
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Invalid API Key" });
                return;
            }

            // 3. If the controller specified allowed vendors, check if our matched vendor is in the list
            if (_allowedVendors != null && _allowedVendors.Length > 0)
            {
                // We check the matchedVendor.Key (e.g., "VendorA") against the list
                if (!_allowedVendors.Contains(matchedVendor.Key, StringComparer.OrdinalIgnoreCase))
                {
                    context.Result = new UnauthorizedObjectResult(new
                    {
                        error = $"Access denied. {matchedVendor.Key} does not have permission for this endpoint."
                    });
                    return;
                }
            }

            // 4. Access Granted
            await next();
        }
    }
}