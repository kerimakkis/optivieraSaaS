using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Optiviera.Data;
using Optiviera.Models;
using System.Security.Cryptography;
using System.Text;

namespace Optiviera.Scripts
{
    public class CreateInitialDatabase
    {
        public static async Task CreateDatabaseAsync(string connectionString)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Create initial data
            await CreateInitialDataAsync(context);
        }

        private static async Task CreateInitialDataAsync(ApplicationDbContext context)
        {
            // Create admin user
            var adminUser = new WaveUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin@optiviera.local",
                Email = "admin@optiviera.local",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true,
                NormalizedUserName = "ADMIN@OPTIVIERA.LOCAL",
                NormalizedEmail = "ADMIN@OPTIVIERA.LOCAL",
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Hash password
            var passwordHasher = new PasswordHasher<WaveUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            // Check if admin user already exists
            var existingAdmin = await context.Users.FirstOrDefaultAsync(u => u.Email == adminUser.Email);
            if (existingAdmin == null)
            {
                context.Users.Add(adminUser);
            }

            // Create roles
            var roles = new[] { "Admin", "Manager", "Employee" };
            foreach (var roleName in roles)
            {
                var existingRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (existingRole == null)
                {
                    var role = new IdentityRole
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        ConcurrencyStamp = Guid.NewGuid().ToString()
                    };
                    context.Roles.Add(role);
                }
            }

            // Assign admin role to admin user
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null && existingAdmin == null)
            {
                var userRole = new IdentityUserRole<string>
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                };
                context.UserRoles.Add(userRole);
            }

            // Create priorities
            var priorities = new[]
            {
                new Priority { Name = "Acil" },
                new Priority { Name = "Yüksek" },
                new Priority { Name = "Orta" },
                new Priority { Name = "Düşük" }
            };

            foreach (var priority in priorities)
            {
                var existingPriority = await context.Priorities.FirstOrDefaultAsync(p => p.Name == priority.Name);
                if (existingPriority == null)
                {
                    context.Priorities.Add(priority);
                }
            }

            // Create initial trial license
            var machineId = GenerateMachineId();
            var existingLicense = await context.Licenses.FirstOrDefaultAsync(l => l.MachineId == machineId);
            if (existingLicense == null)
            {
                var trialLicense = new License
                {
                    LicenseKey = "TRIAL-" + Guid.NewGuid().ToString("N")[..16].ToUpper(),
                    MachineId = machineId,
                    LicenseType = "Trial",
                    ActivationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(365), // 1 year trial
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    LastValidated = DateTime.UtcNow
                };
                context.Licenses.Add(trialLicense);
            }

            await context.SaveChangesAsync();
        }

        private static string GenerateMachineId()
        {
            try
            {
                // Get hardware information
                var machineName = Environment.MachineName;
                var osVersion = Environment.OSVersion.ToString();
                var processorCount = Environment.ProcessorCount.ToString();
                var userName = Environment.UserName;

                // Create a unique identifier based on hardware
                var combined = $"{machineName}-{osVersion}-{processorCount}-{userName}";
                
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    return Convert.ToBase64String(hash)[..16]; // Take first 16 characters
                }
            }
            catch (Exception)
            {
                return Guid.NewGuid().ToString("N")[..16]; // Fallback to GUID
            }
        }
    }
}
