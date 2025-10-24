using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<WaveUser> _userManager;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            ApplicationDbContext context,
            UserManager<WaveUser> userManager,
            ILogger<UsersController> logger)
        {
            _context = context;
            _userManager = userManager;
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
        /// Get all users in the current tenant
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(List<UserDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();

            var users = await _context.Users
                .Where(u => u.TenantId == tenantId)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt
                });
            }

            return Ok(userDtos);
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var tenantId = GetTenantId();

            var user = await _context.Users
                .Where(u => u.Id == id && u.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            });
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();

            // Check tenant user limit
            var tenant = await _context.Tenants
                .Include(t => t.Users)
                .FirstOrDefaultAsync(t => t.Id == tenantId);

            if (tenant == null)
            {
                return BadRequest(new { message = "Tenant not found" });
            }

            if (tenant.Users.Count >= tenant.MaxUsers)
            {
                return BadRequest(new { message = $"Maximum user limit ({tenant.MaxUsers}) reached for this tenant" });
            }

            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            var user = new WaveUser
            {
                TenantId = tenantId,
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Failed to create user: {errors}" });
            }

            // Assign role
            var role = request.Role ?? "Employee";
            await _userManager.AddToRoleAsync(user, role);

            _logger.LogInformation("User created: {UserId} - {Email} with role {Role} for TenantId: {TenantId}",
                user.Id, user.Email, role, tenantId);

            var roles = await _userManager.GetRolesAsync(user);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            });
        }

        /// <summary>
        /// Update an existing user (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = GetTenantId();

            var user = await _context.Users
                .Where(u => u.Id == id && u.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Update basic info
            if (!string.IsNullOrEmpty(request.FirstName))
                user.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                user.LastName = request.LastName;

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    return BadRequest(new { message = "Email already in use" });
                }

                user.Email = request.Email;
                user.UserName = request.Email;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Failed to update user: {errors}" });
            }

            // Update role if provided
            if (!string.IsNullOrEmpty(request.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            _logger.LogInformation("User updated: {UserId} for TenantId: {TenantId}", id, tenantId);

            return NoContent();
        }

        /// <summary>
        /// Delete a user (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            var tenantId = GetTenantId();

            var user = await _context.Users
                .Where(u => u.Id == id && u.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Prevent deleting yourself
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == id)
            {
                return BadRequest(new { message = "Cannot delete your own account" });
            }

            // Check if user has tickets assigned
            var hasTickets = await _context.Tickets
                .AnyAsync(t => t.TechnicianId == id || t.SupportId == id);

            if (hasTickets)
            {
                return BadRequest(new { message = "Cannot delete user with assigned tickets. Please reassign tickets first." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = $"Failed to delete user: {errors}" });
            }

            _logger.LogInformation("User deleted: {UserId} for TenantId: {TenantId}", id, tenantId);

            return NoContent();
        }
    }
}
