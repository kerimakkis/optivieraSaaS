using Optiviera.Models.Enums;

namespace Optiviera.DTOs
{
    public class TicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public PriorityDto? Priority { get; set; }
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = string.Empty;
        public string CustomerState { get; set; } = string.Empty;
        public string CustomerZip { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Schedule { get; set; }
        public bool IsArchived { get; set; }
        public string? TechnicianName { get; set; }
        public string? SupportName { get; set; }
    }

    public class TicketDetailDto : TicketDto
    {
        public List<CommentDto> Comments { get; set; } = new();
    }

    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string CustomerCity { get; set; } = string.Empty;
        public string CustomerState { get; set; } = string.Empty;
        public string CustomerZip { get; set; } = string.Empty;
        public TicketStatus? Status { get; set; }
        public int PriorityId { get; set; }
        public string? TechnicianId { get; set; }
        public string? SupportId { get; set; }
        public DateTimeOffset? Schedule { get; set; }
    }

    public class UpdateTicketRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus? Status { get; set; }
        public int? PriorityId { get; set; }
        public string? TechnicianId { get; set; }
        public string? SupportId { get; set; }
        public DateTimeOffset? Schedule { get; set; }
        public bool? IsArchived { get; set; }
    }
}
