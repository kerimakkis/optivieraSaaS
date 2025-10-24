using Optiviera.Models;

namespace Optiviera.Services.Interfaces
{
    public interface ILicenseService
    {
        Task<bool> ValidateLicenseAsync();
        Task<int> GetTrialDaysRemainingAsync();
        Task<bool> IsInGracePeriodAsync();
        Task<string> GenerateMachineIdAsync();
        Task<bool> ActivateLicenseAsync(string licenseKey);
        Task<bool> CheckOnlineLicenseStatusAsync();
        Task<License?> GetCurrentLicenseAsync();
        Task<bool> IsLicenseValidAsync();
        Task<DateTime?> GetExpiryDateAsync();
        Task<bool> IsTrialExpiredAsync();
        
        // Additional methods needed by LicenseController
        string GetMachineId();
        Task<License?> GetLicenseAsync(string machineId);
        Task<bool> ValidateLicenseAsync(string machineId);
        Task<bool> CreateTrialLicenseAsync();
    }
}
