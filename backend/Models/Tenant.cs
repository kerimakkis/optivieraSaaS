using System.ComponentModel.DataAnnotations;

namespace Optiviera.Models
{
    public class Tenant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? SubDomain { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string ContactEmail { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? ContactPhone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int MaxUsers { get; set; } = 3; // Default: Starter Plan

        [MaxLength(50)]
        public string PlanType { get; set; } = "Starter"; // Free, Starter, Business, Enterprise

        // Navigation Properties
        public virtual ICollection<WaveUser> Users { get; set; } = new List<WaveUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Priority> Priorities { get; set; } = new List<Priority>();
    }
}
