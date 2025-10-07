using System.ComponentModel.DataAnnotations;

namespace Optiviera.Models
{
    public class LicenseHistory
    {
        public int Id { get; set; }
        
        public int LicenseId { get; set; }
        
        public License? License { get; set; }
        
        [Required]
        [StringLength(25)]
        public string LicenseKey { get; set; } = string.Empty;
        
        [Required]
        [StringLength(64)]
        public string MachineId { get; set; } = string.Empty;
        
        public DateTime ValidationDate { get; set; } = DateTime.UtcNow;
        
        public bool IsValid { get; set; }
        
        [StringLength(500)]
        public string? ValidationResult { get; set; }
        
        [StringLength(45)]
        public string? IpAddress { get; set; }
        
        [StringLength(200)]
        public string? UserAgent { get; set; }
    }
}
