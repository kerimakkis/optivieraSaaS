using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Optiviera.Models;

namespace Optiviera.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<WaveUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create Roles
            string[] roleNames = { "Admin", "Manager", "Employee" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create Demo Tenant if no tenants exist
            if (!await context.Tenants.AnyAsync())
            {
                var demoTenant = new Tenant
                {
                    CompanyName = "Demo Company",
                    ContactEmail = "demo@optiviera.com",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    MaxUsers = 10,
                    PlanType = "Demo"
                };

                context.Tenants.Add(demoTenant);
                await context.SaveChangesAsync();

                // Create Demo Admin User
                var adminUser = new WaveUser
                {
                    TenantId = demoTenant.Id,
                    UserName = "admin@optiviera.com",
                    Email = "admin@optiviera.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                // Create Demo Priorities for this tenant
                var priorities = new[]
                {
                    new Priority { TenantId = demoTenant.Id, Name = "Low" },
                    new Priority { TenantId = demoTenant.Id, Name = "Normal" },
                    new Priority { TenantId = demoTenant.Id, Name = "High" },
                    new Priority { TenantId = demoTenant.Id, Name = "Urgent" }
                };

                context.Priorities.AddRange(priorities);
                await context.SaveChangesAsync();

                Console.WriteLine("Demo tenant created successfully!");
                Console.WriteLine($"Email: admin@optiviera.com");
                Console.WriteLine($"Password: Admin123!");
                Console.WriteLine($"TenantId: {demoTenant.Id}");
            }
        }
    }
}
