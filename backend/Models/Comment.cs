using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Optiviera.Models
{
    public class Comment
    {
        public int Id { get; set; }

        // Multi-Tenant Support
        [Required]
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        [Display(Name = "Ticket Id")]
        public int TicketId { get; set; }

        [Display(Name = "Comment")]
        [Required]
        public string Note { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Comment Date")]
        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        [Display(Name = "User")]
        [Required]
        public string UserId { get; set; } = string.Empty;

        // Navigation Properties
        public virtual Ticket? Ticket { get; set; }

        public virtual WaveUser? User { get; set; }
    }
}
