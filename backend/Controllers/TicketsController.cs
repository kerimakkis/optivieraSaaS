using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.Models.Enums;
using System.Security.Claims;

namespace Optiviera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ApplicationDbContext context, ILogger<TicketsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetTenantId()
        {
            var tenantId = HttpContext.Items["TenantId"];
            if (tenantId == null)
            {
                throw new UnauthorizedAccessException("Tenant ID not found");
            }
            return (int)tenantId;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found");
        }

        /// <summary>
        /// Get all tickets for the current tenant
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<TicketDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();

            var tickets = await _context.Tickets
                .Where(t => t.TenantId == tenantId)
                .Include(t => t.Priority)
                .Include(t => t.Technician)
                .Include(t => t.Support)
                .OrderByDescending(t => t.Created)
                .Select(t => new TicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Priority = t.Priority != null ? new PriorityDto
                    {
                        Id = t.Priority.Id,
                        Name = t.Priority.Name
                    } : null,
                    CustomerFirstName = t.CFirstName,
                    CustomerLastName = t.CLastName,
                    CustomerPhone = t.CPhone,
                    CustomerAddress = t.CAddress,
                    CustomerCity = t.CCity,
                    CustomerState = t.CState,
                    CustomerZip = t.CZip,
                    Created = t.Created,
                    Schedule = t.Schedule,
                    IsArchived = t.IsArchived,
                    TechnicianName = t.Technician != null ? t.Technician.FullName : null,
                    SupportName = t.Support != null ? t.Support.FullName : null
                })
                .ToListAsync();

            return Ok(tickets);
        }

        /// <summary>
        /// Get a specific ticket by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TicketDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var tenantId = GetTenantId();

            var ticket = await _context.Tickets
                .Where(t => t.TenantId == tenantId && t.Id == id)
                .Include(t => t.Priority)
                .Include(t => t.Technician)
                .Include(t => t.Support)
                .Include(t => t.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            var result = new TicketDetailDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority != null ? new PriorityDto
                {
                    Id = ticket.Priority.Id,
                    Name = ticket.Priority.Name
                } : null,
                CustomerFirstName = ticket.CFirstName,
                CustomerLastName = ticket.CLastName,
                CustomerPhone = ticket.CPhone,
                CustomerAddress = ticket.CAddress,
                CustomerCity = ticket.CCity,
                CustomerState = ticket.CState,
                CustomerZip = ticket.CZip,
                Created = ticket.Created,
                Schedule = ticket.Schedule,
                IsArchived = ticket.IsArchived,
                TechnicianName = ticket.Technician != null ? ticket.Technician.FullName : null,
                SupportName = ticket.Support != null ? ticket.Support.FullName : null,
                Comments = ticket.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Note = c.Note,
                    Created = c.Created,
                    UserName = c.User != null ? c.User.FullName : "Unknown"
                }).ToList()
            };

            return Ok(result);
        }

        /// <summary>
        /// Create a new ticket
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();

            var ticket = new Ticket
            {
                TenantId = tenantId,
                Title = request.Title,
                Description = request.Description,
                CFirstName = request.CustomerFirstName,
                CLastName = request.CustomerLastName,
                CPhone = request.CustomerPhone,
                CAddress = request.CustomerAddress,
                CCity = request.CustomerCity,
                CState = request.CustomerState,
                CZip = request.CustomerZip,
                Status = request.Status ?? TicketStatus.Open,
                PriorityId = request.PriorityId,
                TechnicianId = request.TechnicianId,
                SupportId = request.SupportId,
                Created = DateTimeOffset.UtcNow,
                Schedule = request.Schedule ?? DateTimeOffset.UtcNow,
                IsArchived = false
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ticket created: {TicketId} by TenantId: {TenantId}", ticket.Id, tenantId);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                CustomerFirstName = ticket.CFirstName,
                CustomerLastName = ticket.CLastName,
                Created = ticket.Created
            });
        }

        /// <summary>
        /// Update an existing ticket
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();

            var ticket = await _context.Tickets
                .Where(t => t.TenantId == tenantId && t.Id == id)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            ticket.Title = request.Title ?? ticket.Title;
            ticket.Description = request.Description ?? ticket.Description;
            ticket.Status = request.Status ?? ticket.Status;
            ticket.PriorityId = request.PriorityId ?? ticket.PriorityId;
            ticket.TechnicianId = request.TechnicianId ?? ticket.TechnicianId;
            ticket.SupportId = request.SupportId ?? ticket.SupportId;
            ticket.Schedule = request.Schedule ?? ticket.Schedule;
            ticket.IsArchived = request.IsArchived ?? ticket.IsArchived;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Ticket updated: {TicketId} by TenantId: {TenantId}", id, tenantId);

            return NoContent();
        }

        /// <summary>
        /// Delete a ticket
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var tenantId = GetTenantId();

            var ticket = await _context.Tickets
                .Where(t => t.TenantId == tenantId && t.Id == id)
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Ticket deleted: {TicketId} by TenantId: {TenantId}", id, tenantId);

            return NoContent();
        }
    }

    // DTOs
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

    public class PriorityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CommentDto
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTimeOffset Created { get; set; }
        public string UserName { get; set; } = string.Empty;
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
