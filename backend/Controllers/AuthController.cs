using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optiviera.Services.Interfaces;
using Optiviera.DTOs;

namespace Optiviera.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new tenant and admin user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid request data" });
            }

            var (success, token, user, message) = await _authService.RegisterAsync(
                request.CompanyName,
                request.Email,
                request.Password,
                request.FirstName,
                request.LastName
            );

            if (!success)
            {
                _logger.LogWarning("Registration failed for email {Email}: {Message}", request.Email, message);
                return BadRequest(new ErrorResponse { Message = message });
            }

            _logger.LogInformation("User registered successfully: {Email}, TenantId: {TenantId}",
                user!.Email, user.TenantId);

            return Ok(new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TenantId = user.TenantId
                }
            });
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Invalid request data" });
            }

            var (success, token, user, message) = await _authService.LoginAsync(
                request.Email,
                request.Password
            );

            if (!success)
            {
                _logger.LogWarning("Login failed for email {Email}: {Message}", request.Email, message);
                return Unauthorized(new ErrorResponse { Message = message });
            }

            _logger.LogInformation("User logged in successfully: {Email}, TenantId: {TenantId}",
                user!.Email, user.TenantId);

            return Ok(new AuthResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    TenantId = user.TenantId
                }
            });
        }

        /// <summary>
        /// Get current user information (requires authentication)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var firstName = User.FindFirst("FirstName")?.Value;
            var lastName = User.FindFirst("LastName")?.Value;
            var tenantId = User.FindFirst("TenantId")?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            return Ok(new UserDto
            {
                Id = userId,
                Email = email!,
                FirstName = firstName!,
                LastName = lastName!,
                TenantId = int.Parse(tenantId)
            });
        }
    }
}
