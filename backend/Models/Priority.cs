using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optiviera.Models
{
    public class Priority
    {
        public int Id { get; set; }

        // Multi-Tenant Support
        [Required]
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        [Display(Name = "Priority Name")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation Properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
