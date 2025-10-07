using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optiviera.Models;
using Optiviera.Services.Interfaces;

namespace Optiviera.Controllers
{
    public class LicenseController : Controller
    {
        private readonly ILicenseService _licenseService;
        private readonly ILogger<LicenseController> _logger;

        public LicenseController(ILicenseService licenseService, ILogger<LicenseController> logger)
        {
            _licenseService = licenseService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                // Simple test for now
                return Content("License Controller Working!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading license page");
                return Content($"Error: {ex.Message}");
            }
        }

        public async Task<IActionResult> Info()
        {
            try
            {
                var machineId = _licenseService.GetMachineId();
                var license = await _licenseService.GetLicenseAsync(machineId);
                
                // If no license exists, create a trial license
                if (license == null)
                {
                    await _licenseService.CreateTrialLicenseAsync();
                    license = await _licenseService.GetLicenseAsync(machineId);
                }
                
                var isValid = await _licenseService.ValidateLicenseAsync(machineId);
                
                var viewModel = new LicenseViewModel
                {
                    License = license,
                    IsLicenseValid = isValid,
                    ExpiryDate = license?.ExpiryDate,
                    TrialDaysRemaining = license?.ExpiryDate != null ? 
                        Math.Max(0, (int)(license.ExpiryDate - DateTime.Now).TotalDays) : 0,
                    IsInGracePeriod = license?.GracePeriodEnd != null && 
                        DateTime.Now <= license.GracePeriodEnd
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading license info page");
                return View(new LicenseViewModel 
                { 
                    IsLicenseValid = false,
                    TrialDaysRemaining = 0
                });
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
