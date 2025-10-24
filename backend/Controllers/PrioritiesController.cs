using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.DTOs;

namespace Optiviera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PrioritiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PrioritiesController> _logger;

        public PrioritiesController(ApplicationDbContext context, ILogger<PrioritiesController> logger)
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

        /// <summary>
        /// Get all priorities for the current tenant
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PriorityDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();

            var priorities = await _context.Priorities
                .Where(p => p.TenantId == tenantId)
                .OrderBy(p => p.Name)
                .Select(p => new PriorityDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

            return Ok(priorities);
        }

        /// <summary>
        /// Get a specific priority by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PriorityDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var tenantId = GetTenantId();

            var priority = await _context.Priorities
                .Where(p => p.Id == id && p.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (priority == null)
            {
                return NotFound(new { message = "Priority not found" });
            }

            return Ok(new PriorityDto
            {
                Id = priority.Id,
                Name = priority.Name
            });
        }

        /// <summary>
        /// Create a new priority (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PriorityDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePriorityRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();

            // Check if priority name already exists for this tenant
            var exists = await _context.Priorities
                .AnyAsync(p => p.TenantId == tenantId && p.Name == request.Name);

            if (exists)
            {
                return BadRequest(new { message = "Priority with this name already exists" });
            }

            var priority = new Priority
            {
                TenantId = tenantId,
                Name = request.Name
            };

            _context.Priorities.Add(priority);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Priority created: {PriorityId} - {PriorityName} for TenantId: {TenantId}",
                priority.Id, priority.Name, tenantId);

            return CreatedAtAction(nameof(GetById), new { id = priority.Id }, new PriorityDto
            {
                Id = priority.Id,
                Name = priority.Name
            });
        }

        /// <summary>
        /// Update an existing priority (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePriorityRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();

            var priority = await _context.Priorities
                .Where(p => p.Id == id && p.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (priority == null)
            {
                return NotFound(new { message = "Priority not found" });
            }

            // Check if new name already exists for this tenant
            if (request.Name != null && request.Name != priority.Name)
            {
                var exists = await _context.Priorities
                    .AnyAsync(p => p.TenantId == tenantId && p.Name == request.Name && p.Id != id);

                if (exists)
                {
                    return BadRequest(new { message = "Priority with this name already exists" });
                }

                priority.Name = request.Name;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Priority updated: {PriorityId} for TenantId: {TenantId}", id, tenantId);

            return NoContent();
        }

        /// <summary>
        /// Delete a priority (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            var tenantId = GetTenantId();

            var priority = await _context.Priorities
                .Where(p => p.Id == id && p.TenantId == tenantId)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync();

            if (priority == null)
            {
                return NotFound(new { message = "Priority not found" });
            }

            // Check if priority is in use
            if (priority.Tickets.Any())
            {
                return BadRequest(new { message = "Cannot delete priority that is assigned to tickets" });
            }

            _context.Priorities.Remove(priority);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Priority deleted: {PriorityId} for TenantId: {TenantId}", id, tenantId);

            return NoContent();
        }
    }
}
