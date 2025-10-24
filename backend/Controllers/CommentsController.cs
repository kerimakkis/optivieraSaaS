using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.DTOs;
using System.Security.Claims;

namespace Optiviera.Controllers
{
    [ApiController]
    [Route("api/tickets/{ticketId}/[controller]")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ApplicationDbContext context, ILogger<CommentsController> logger)
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
        /// Get all comments for a specific ticket
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<CommentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var tenantId = GetTenantId();

            // Verify ticket exists and belongs to tenant
            var ticketExists = await _context.Tickets
                .AnyAsync(t => t.Id == ticketId && t.TenantId == tenantId);

            if (!ticketExists)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            var comments = await _context.Comments
                .Where(c => c.TicketId == ticketId && c.TenantId == tenantId)
                .Include(c => c.User)
                .OrderBy(c => c.Created)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Note = c.Note,
                    Created = c.Created,
                    UserName = c.User != null ? c.User.FullName : "Unknown",
                    UserId = c.UserId
                })
                .ToListAsync();

            return Ok(comments);
        }

        /// <summary>
        /// Get a specific comment by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetComment(int ticketId, int id)
        {
            var tenantId = GetTenantId();

            var comment = await _context.Comments
                .Where(c => c.Id == id && c.TicketId == ticketId && c.TenantId == tenantId)
                .Include(c => c.User)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            return Ok(new CommentDto
            {
                Id = comment.Id,
                Note = comment.Note,
                Created = comment.Created,
                UserName = comment.User != null ? comment.User.FullName : "Unknown",
                UserId = comment.UserId
            });
        }

        /// <summary>
        /// Create a new comment on a ticket
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateComment(int ticketId, [FromBody] CreateCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();
            var userId = GetUserId();

            // Verify ticket exists and belongs to tenant
            var ticketExists = await _context.Tickets
                .AnyAsync(t => t.Id == ticketId && t.TenantId == tenantId);

            if (!ticketExists)
            {
                return NotFound(new { message = "Ticket not found" });
            }

            var comment = new Comment
            {
                TenantId = tenantId,
                TicketId = ticketId,
                UserId = userId,
                Note = request.Note,
                Created = DateTimeOffset.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment created: {CommentId} for Ticket: {TicketId} by User: {UserId}",
                comment.Id, ticketId, userId);

            // Reload with user info
            await _context.Entry(comment).Reference(c => c.User).LoadAsync();

            return CreatedAtAction(nameof(GetComment),
                new { ticketId = ticketId, id = comment.Id },
                new CommentDto
                {
                    Id = comment.Id,
                    Note = comment.Note,
                    Created = comment.Created,
                    UserName = comment.User != null ? comment.User.FullName : "Unknown",
                    UserId = comment.UserId
                });
        }

        /// <summary>
        /// Update an existing comment
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateComment(int ticketId, int id, [FromBody] UpdateCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();
            var userId = GetUserId();

            var comment = await _context.Comments
                .Where(c => c.Id == id && c.TicketId == ticketId && c.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            // Only allow user to edit their own comments (or admin)
            if (comment.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            comment.Note = request.Note ?? comment.Note;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment updated: {CommentId} by User: {UserId}", id, userId);

            return NoContent();
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteComment(int ticketId, int id)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            var comment = await _context.Comments
                .Where(c => c.Id == id && c.TicketId == ticketId && c.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (comment == null)
            {
                return NotFound(new { message = "Comment not found" });
            }

            // Only allow user to delete their own comments (or admin)
            if (comment.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Comment deleted: {CommentId} by User: {UserId}", id, userId);

            return NoContent();
        }
    }
}
