using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Optiviera.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<WaveUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<WaveUser> userManager,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        public string GenerateJwtToken(WaveUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryInDays = int.Parse(jwtSettings["ExpiryInDays"] ?? "7");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("TenantId", user.TenantId.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName)
            };

            // Add roles
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expiryInDays),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(bool Success, string Token, WaveUser? User, string Message)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return (false, string.Empty, null, "Invalid email or password");
            }

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (!result)
            {
                return (false, string.Empty, null, "Invalid email or password");
            }

            var token = GenerateJwtToken(user);
            return (true, token, user, "Login successful");
        }

        public async Task<(bool Success, string Token, WaveUser? User, string Message)> RegisterAsync(
            string companyName,
            string email,
            string password,
            string firstName,
            string lastName)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return (false, string.Empty, null, "User with this email already exists");
            }

            // Create tenant
            var tenant = new Tenant
            {
                CompanyName = companyName,
                ContactEmail = email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                MaxUsers = 3, // Starter plan
                PlanType = "Starter"
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Create user
            var user = new WaveUser
            {
                TenantId = tenant.Id,
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                // Rollback tenant creation
                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, string.Empty, null, $"Failed to create user: {errors}");
            }

            // Assign Admin role
            await _userManager.AddToRoleAsync(user, "Admin");

            var token = GenerateJwtToken(user);
            return (true, token, user, "Registration successful");
        }
    }
}
