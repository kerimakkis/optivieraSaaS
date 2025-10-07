using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optiviera.Models;
using Optiviera.Services.Interfaces;

namespace Optiviera.Controllers
{
    [Authorize]
    public class LicenseController : Controller
    {
        private readonly ILicenseService _licenseService;
        private readonly ILogger<LicenseController> _logger;

        public LicenseController(ILicenseService licenseService, ILogger<LicenseController> logger)
        {
            _licenseService = licenseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var license = await _licenseService.GetCurrentLicenseAsync();
                var daysRemaining = await _licenseService.GetTrialDaysRemainingAsync();
                var isInGracePeriod = await _licenseService.IsInGracePeriodAsync();
                var isLicenseValid = await _licenseService.IsLicenseValidAsync();

                var model = new LicenseViewModel
                {
                    License = license,
                    TrialDaysRemaining = daysRemaining,
                    IsInGracePeriod = isInGracePeriod,
                    IsLicenseValid = isLicenseValid,
                    ExpiryDate = await _licenseService.GetExpiryDateAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading license page");
                return View(new LicenseViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Activate(string licenseKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(licenseKey))
                {
                    TempData["Error"] = "Please enter a valid license key.";
                    return RedirectToAction(nameof(Index));
                }

                var success = await _licenseService.ActivateLicenseAsync(licenseKey);
                
                if (success)
                {
                    TempData["Success"] = "License activated successfully!";
                    _logger.LogInformation("License activated successfully: {LicenseKey}", licenseKey);
                }
                else
                {
                    TempData["Error"] = "Invalid license key. Please check and try again.";
                    _logger.LogWarning("Failed to activate license: {LicenseKey}", licenseKey);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating license: {LicenseKey}", licenseKey);
                TempData["Error"] = "An error occurred while activating the license. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Expired()
        {
            return View();
        }

        public IActionResult Purchase()
        {
            // Redirect to akkistech.com purchase page
            return Redirect("https://akkistech.com/optiviera/purchase");
        }
    }

    public class LicenseViewModel
    {
        public License? License { get; set; }
        public int TrialDaysRemaining { get; set; }
        public bool IsInGracePeriod { get; set; }
        public bool IsLicenseValid { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
