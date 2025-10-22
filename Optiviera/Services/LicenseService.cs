using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Optiviera.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<LicenseService> _logger;
        private readonly IMemoryCache _cache;
        private readonly string _machineId;
        private const string LICENSE_CACHE_KEY = "CurrentLicense";
        private const int CACHE_DURATION_MINUTES = 10;

        public LicenseService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<LicenseService> logger,
            IMemoryCache cache)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _cache = cache;
            _machineId = GenerateMachineId();
        }

        public async Task<bool> ValidateLicenseAsync()
        {
            try
            {
                var license = await GetCurrentLicenseAsync();
                if (license == null)
                {
                    _logger.LogWarning("No license found");
                    return false;
                }

                // Check if license is active
                if (!license.IsActive)
                {
                    _logger.LogWarning("License is not active");
                    return false;
                }

                // Check if license has expired
                if (DateTime.UtcNow > license.ExpiryDate)
                {
                    // For trial licenses, allow some grace period
                    if (license.LicenseType == "Trial")
                    {
                        // Trial licenses get 7 days grace period
                        var gracePeriodEnd = license.ExpiryDate.AddDays(7);
                        if (DateTime.UtcNow <= gracePeriodEnd)
                        {
                            _logger.LogInformation("Trial license expired but in grace period");
                            return true;
                        }
                    }
                    else
                    {
                        // Check if we're in grace period for paid licenses
                        if (license.GracePeriodEnd.HasValue && DateTime.UtcNow <= license.GracePeriodEnd.Value)
                        {
                            _logger.LogInformation("License expired but in grace period");
                            return true;
                        }
                    }
                    
                    _logger.LogWarning("License has expired");
                    return false;
                }

                // Update last validated (throttled - only if older than 1 hour)
                if (license.LastValidated == null ||
                    (DateTime.UtcNow - license.LastValidated.Value).TotalHours >= 1)
                {
                    license.LastValidated = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                // Note: Validation logging disabled for performance
                // Re-enable if audit trail is needed
                // await LogValidationAttemptAsync(license, true, "License validated successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating license");
                return false;
            }
        }

        public async Task<int> GetTrialDaysRemainingAsync()
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null || license.LicenseType != "Trial")
                return 0;

            var daysRemaining = (license.ExpiryDate - DateTime.UtcNow).Days;
            return Math.Max(0, daysRemaining);
        }

        public async Task<bool> IsInGracePeriodAsync()
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null || !license.GracePeriodEnd.HasValue)
                return false;

            return DateTime.UtcNow <= license.GracePeriodEnd.Value;
        }

        public async Task<string> GenerateMachineIdAsync()
        {
            return GenerateMachineId();
        }

        private static string GenerateMachineId()
        {
            try
            {
                // Get hardware information
                var machineName = Environment.MachineName;
                var osVersion = Environment.OSVersion.ToString();
                var processorCount = Environment.ProcessorCount.ToString();
                var userName = Environment.UserName;

                // Create a unique identifier based on hardware
                var combined = $"{machineName}-{osVersion}-{processorCount}-{userName}";
                
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    return Convert.ToBase64String(hash)[..16]; // Take first 16 characters
                }
            }
            catch (Exception)
            {
                return Guid.NewGuid().ToString("N")[..16]; // Fallback to GUID
            }
        }

        public async Task<bool> ActivateLicenseAsync(string licenseKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(licenseKey))
                    return false;

                // Validate license key format
                if (!IsValidLicenseKeyFormat(licenseKey))
                {
                    _logger.LogWarning("Invalid license key format: {LicenseKey}", licenseKey);
                    return false;
                }

                var machineId = await GenerateMachineIdAsync();
                
                // Check if license key is valid (basic validation)
                var isValidKey = await ValidateLicenseKeyAsync(licenseKey);
                if (!isValidKey)
                {
                    _logger.LogWarning("Invalid license key: {LicenseKey}", licenseKey);
                    return false;
                }

                // Create or update license
                var existingLicense = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.MachineId == machineId);

                if (existingLicense != null)
                {
                    existingLicense.LicenseKey = licenseKey;
                    existingLicense.LicenseType = "Full";
                    existingLicense.ActivationDate = DateTime.UtcNow;
                    existingLicense.ExpiryDate = DateTime.UtcNow.AddYears(1); // 1 year from activation
                    existingLicense.IsActive = true;
                    existingLicense.GracePeriodEnd = null;
                    existingLicense.LastValidated = DateTime.UtcNow;
                }
                else
                {
                    var newLicense = new License
                    {
                        LicenseKey = licenseKey,
                        MachineId = machineId,
                        LicenseType = "Full",
                        ActivationDate = DateTime.UtcNow,
                        ExpiryDate = DateTime.UtcNow.AddYears(1),
                        IsActive = true,
                        LastValidated = DateTime.UtcNow
                    };
                    _context.Licenses.Add(newLicense);
                }

                await _context.SaveChangesAsync();

                // Invalidate cache after license update
                _cache.Remove(LICENSE_CACHE_KEY);

                _logger.LogInformation("License activated successfully: {LicenseKey}", licenseKey);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating license: {LicenseKey}", licenseKey);
                return false;
            }
        }

        public async Task<bool> CheckOnlineLicenseStatusAsync()
        {
            try
            {
                // This would check against akkistech.com API
                // For now, return true (offline mode)
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking online license status");
                return false;
            }
        }

        public async Task<License?> GetCurrentLicenseAsync()
        {
            try
            {
                // Try to get from cache first
                if (_cache.TryGetValue(LICENSE_CACHE_KEY, out License? cachedLicense))
                {
                    return cachedLicense;
                }

                // Not in cache, fetch from database
                var machineId = await GenerateMachineIdAsync();
                var license = await _context.Licenses
                    .AsNoTracking() // Performance: don't track entities we're just reading
                    .FirstOrDefaultAsync(l => l.MachineId == machineId);

                // Cache the result for 10 minutes
                if (license != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(CACHE_DURATION_MINUTES));
                    _cache.Set(LICENSE_CACHE_KEY, license, cacheOptions);
                }

                return license;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current license");
                return null;
            }
        }

        public async Task<bool> IsLicenseValidAsync()
        {
            return await ValidateLicenseAsync();
        }

        public async Task<DateTime?> GetExpiryDateAsync()
        {
            var license = await GetCurrentLicenseAsync();
            return license?.ExpiryDate;
        }

        public async Task<bool> IsTrialExpiredAsync()
        {
            var license = await GetCurrentLicenseAsync();
            if (license == null || license.LicenseType != "Trial")
                return false;

            return DateTime.UtcNow > license.ExpiryDate;
        }

        private bool IsValidLicenseKeyFormat(string licenseKey)
        {
            // Format: OPTV-XXXX-XXXX-XXXX-XXXX
            if (string.IsNullOrWhiteSpace(licenseKey) || licenseKey.Length != 25)
                return false;

            var parts = licenseKey.Split('-');
            if (parts.Length != 5)
                return false;

            if (parts[0] != "OPTV")
                return false;

            // Check if all parts are alphanumeric
            foreach (var part in parts.Skip(1))
            {
                if (part.Length != 4 || !part.All(c => char.IsLetterOrDigit(c)))
                    return false;
            }

            return true;
        }

        private async Task<bool> ValidateLicenseKeyAsync(string licenseKey)
        {
            // Basic validation - in production, this would validate against a server
            // For now, just check format and basic checksum
            if (!IsValidLicenseKeyFormat(licenseKey))
                return false;

            // Extract and validate checksum
            var parts = licenseKey.Split('-');
            var data = string.Join("", parts.Skip(1).Take(4));
            var checksum = parts[4];

            // Simple checksum validation (in production, use proper cryptographic validation)
            var calculatedChecksum = CalculateSimpleChecksum(data);
            return checksum == calculatedChecksum;
        }

        private string CalculateSimpleChecksum(string data)
        {
            var sum = data.Sum(c => c);
            return (sum % 10000).ToString("D4");
        }

        private async Task LogValidationAttemptAsync(License license, bool isValid, string result)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var history = new LicenseHistory
                {
                    LicenseId = license.Id,
                    LicenseKey = license.LicenseKey,
                    MachineId = license.MachineId,
                    IsValid = isValid,
                    ValidationResult = result,
                    IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                    UserAgent = httpContext?.Request?.Headers?.UserAgent.ToString()
                };

                _context.LicenseHistories.Add(history);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging validation attempt");
            }
        }

        // Additional methods needed by LicenseController
        public string GetMachineId()
        {
            return _machineId;
        }

        public async Task<License?> GetLicenseAsync(string machineId)
        {
            try
            {
                return await _context.Licenses
                    .FirstOrDefaultAsync(l => l.MachineId == machineId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting license for machine ID: {MachineId}", machineId);
                return null;
            }
        }

        public async Task<bool> ValidateLicenseAsync(string machineId)
        {
            try
            {
                var license = await GetLicenseAsync(machineId);
                if (license == null)
                {
                    _logger.LogWarning("No license found for machine ID: {MachineId}", machineId);
                    return false;
                }

                return await ValidateLicenseAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating license for machine ID: {MachineId}", machineId);
                return false;
            }
        }

        public async Task<bool> CreateTrialLicenseAsync()
        {
            try
            {
                // Check if trial license already exists
                var existingLicense = await _context.Licenses
                    .FirstOrDefaultAsync(l => l.MachineId == _machineId);

                if (existingLicense != null)
                {
                    return true; // Trial already exists
                }

                // Create new trial license
                var trialLicense = new License
                {
                    LicenseKey = $"TRIAL-{_machineId}-{DateTime.UtcNow:yyyyMMdd}",
                    MachineId = _machineId,
                    LicenseType = "Trial",
                    ActivationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(365), // 1 year trial
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastValidated = DateTime.UtcNow,
                    CustomerEmail = "trial@optiviera.com",
                    Notes = "Free trial license - 1 year validity"
                };

                _context.Licenses.Add(trialLicense);
                await _context.SaveChangesAsync();

                // Invalidate cache after license creation
                _cache.Remove(LICENSE_CACHE_KEY);

                _logger.LogInformation("Trial license created for machine ID: {MachineId}", _machineId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trial license");
                return false;
            }
        }
    }
}
