using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Optiviera.Models.Enums;

namespace Optiviera.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        // Multi-Tenant Support
        [Required]
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string CFirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string CLastName { get; set; } = string.Empty;

        [DataType(DataType.PhoneNumber)]
        [Required]
        [Display(Name = "Phone")]
        public string CPhone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Address")]
        public string CAddress { get; set; } = string.Empty;

        [Required]
        [Display(Name = "City")]
        public string CCity { get; set; } = string.Empty;

        [Required]
        [Display(Name = "State")]
        public string CState { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Zip")]
        public string CZip { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Sheduled")]
        public DateTimeOffset Schedule { get; set; }

        [Display(Name = "Archived")]
        public bool IsArchived { get; set; }

        [Display(Name = "Status")]
        public TicketStatus Status { get; set; } = TicketStatus.Open;

        [NotMapped]
        [Display(Name = "Phone Number")]
        public string FormatedPhone { get { return String.Format("{0:(###) ###-####}", Convert.ToInt64(CPhone)); } }
        [NotMapped]
        public string NumberStreet { get { return CAddress.Replace(" ", "+"); } }

        [NotMapped]
        [Display(Name = "Address Link")]
        public string AddressLink { get { return $"https://google.com/maps/search/{NumberStreet}+{CCity}+{CState}+{CZip}"; } }

        [NotMapped]
        [Display(Name = "Formatted Time")]
        public string FormattedTime { get { return Schedule.ToString("MM/dd/yy H:mm EST"); } }


        // Below Referances Priority, Status, Comment

        [Display(Name = "Priority")]
        public int PriorityId { get; set; }

        [Display(Name = "Technician")]
        public string? TechnicianId { get; set; }

        [Display(Name = "Technical Support")]
        public string? SupportId { get; set; }

        // Navigation Properties

        public virtual Priority? Priority { get; set; }

        public virtual WaveUser? Technician { get; set; }

        public virtual WaveUser? Support { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
