using Optiviera.Services.Interfaces;

namespace Optiviera.Middleware
{
    public class LicenseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LicenseMiddleware> _logger;

        public LicenseMiddleware(RequestDelegate next, ILogger<LicenseMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ILicenseService licenseService)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";

            // Skip license validation for license pages and static files
            if (IsLicensePage(path) || IsStaticFile(path) || IsApiEndpoint(path))
            {
                await _next(context);
                return;
            }

            // Check if we already redirected to license page in this session
            if (context.Session.GetString("LicenseRedirected") == "true")
            {
                await _next(context);
                return;
            }

            try
            {
                // Check if license is valid
                var isLicenseValid = await licenseService.IsLicenseValidAsync();
                
                if (!isLicenseValid)
                {
                    // Check if we're in grace period
                    var isInGracePeriod = await licenseService.IsInGracePeriodAsync();
                    
                    if (isInGracePeriod)
                    {
                        // Add grace period warning to response headers
                        context.Response.Headers.Add("X-License-Status", "grace-period");
                        context.Response.Headers.Add("X-Grace-Period-End", 
                            (await licenseService.GetExpiryDateAsync())?.ToString("yyyy-MM-dd") ?? "");
                    }
                    else
                    {
                        // License expired and not in grace period - redirect to license page ONCE
                        _logger.LogWarning("License validation failed, redirecting to license page");
                        context.Session.SetString("LicenseRedirected", "true");
                        context.Response.Redirect("/License/Expired");
                        return;
                    }
                }
                else
                {
                    // License is valid
                    context.Response.Headers.Add("X-License-Status", "valid");
                }

                // Add trial days remaining header if it's a trial license
                var currentLicense = await licenseService.GetCurrentLicenseAsync();
                if (currentLicense?.LicenseType == "Trial")
                {
                    var daysRemaining = await licenseService.GetTrialDaysRemainingAsync();
                    context.Response.Headers.Add("X-Trial-Days-Remaining", daysRemaining.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in license middleware");
                // Continue with request even if license check fails
            }

            await _next(context);
        }

        private static bool IsLicensePage(string path)
        {
            return path.StartsWith("/license", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("/identity/account/login", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("/identity/account/register", StringComparison.OrdinalIgnoreCase) ||
                   path.Equals("/license/expired", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsStaticFile(string path)
        {
            var staticExtensions = new[] { ".css", ".js", ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", ".woff", ".woff2", ".ttf", ".eot" };
            return staticExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsApiEndpoint(string path)
        {
            return path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
        }
    }
}
