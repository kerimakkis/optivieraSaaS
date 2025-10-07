using System.Text.Json;

namespace Optiviera.Services
{
    public class UpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UpdateService> _logger;
        private readonly string _versionCheckUrl = "https://akkistech.com/api/optiviera/version.json";

        public UpdateService(HttpClient httpClient, ILogger<UpdateService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UpdateInfo?> CheckForUpdatesAsync()
        {
            try
            {
                _logger.LogInformation("Checking for updates...");
                
                var response = await _httpClient.GetAsync(_versionCheckUrl);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to check for updates. Status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(json);
                
                if (updateInfo != null)
                {
                    var currentVersion = GetCurrentVersion();
                    var isNewer = IsNewerVersion(updateInfo.Version, currentVersion);
                    
                    if (isNewer)
                    {
                        _logger.LogInformation("Update available: {NewVersion} (current: {CurrentVersion})", 
                            updateInfo.Version, currentVersion);
                        return updateInfo;
                    }
                    else
                    {
                        _logger.LogInformation("No updates available. Current version: {CurrentVersion}", currentVersion);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for updates");
                return null;
            }
        }

        public string GetCurrentVersion()
        {
            // Get version from assembly or configuration
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version?.ToString() ?? "1.0.0";
        }

        private bool IsNewerVersion(string newVersion, string currentVersion)
        {
            try
            {
                var newVersionParts = newVersion.Split('.').Select(int.Parse).ToArray();
                var currentVersionParts = currentVersion.Split('.').Select(int.Parse).ToArray();

                for (int i = 0; i < Math.Max(newVersionParts.Length, currentVersionParts.Length); i++)
                {
                    var newPart = i < newVersionParts.Length ? newVersionParts[i] : 0;
                    var currentPart = i < currentVersionParts.Length ? currentVersionParts[i] : 0;

                    if (newPart > currentPart)
                        return true;
                    if (newPart < currentPart)
                        return false;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing versions: {NewVersion} vs {CurrentVersion}", newVersion, currentVersion);
                return false;
            }
        }

        public string GetDownloadUrl(string platform)
        {
            return $"https://akkistech.com/downloads/optiviera/latest/{platform}";
        }
    }

    public class UpdateInfo
    {
        public string Version { get; set; } = string.Empty;
        public string ReleaseNotes { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string DownloadUrl { get; set; } = string.Empty;
        public bool IsCritical { get; set; }
        public long FileSize { get; set; }
        public string Checksum { get; set; } = string.Empty;
    }
}
