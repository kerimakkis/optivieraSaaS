using System.ComponentModel.DataAnnotations;

namespace Optiviera.Models
{
    public class License
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(25)]
        public string LicenseKey { get; set; } = string.Empty;
        
        [Required]
        [StringLength(64)]
        public string MachineId { get; set; } = string.Empty;
        
        public DateTime ActivationDate { get; set; }
        
        public DateTime ExpiryDate { get; set; }
        
        public bool IsActive { get; set; }
        
        [Required]
        [StringLength(20)]
        public string LicenseType { get; set; } = "Trial"; // Trial, Full
        
        public DateTime? GracePeriodEnd { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastValidated { get; set; }
        
        [StringLength(100)]
        public string? CustomerEmail { get; set; }
        
        [StringLength(200)]
        public string? Notes { get; set; }
    }
}
