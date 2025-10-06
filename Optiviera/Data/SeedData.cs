using Microsoft.AspNetCore.Identity;
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

            // Rolleri oluştur
            string[] roleNames = { "Admin", "Manager", "Employee", "Not Verified" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin kullanıcısı oluştur
            var adminUser = await userManager.FindByNameAsync("admin@optiviera.com");
            if (adminUser == null)
            {
                adminUser = new WaveUser
                {
                    UserName = "admin@optiviera.com",
                    Email = "admin@optiviera.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Serdar kullanıcısı oluştur
            var serdarUser = await userManager.FindByNameAsync("serdar@optiviera.com");
            if (serdarUser == null)
            {
                serdarUser = new WaveUser
                {
                    UserName = "serdar@optiviera.com",
                    Email = "serdar@optiviera.com",
                    FirstName = "Serdar",
                    LastName = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(serdarUser, "Serdar123!");
                await userManager.AddToRoleAsync(serdarUser, "Employee");
            }

            // Manager kullanıcısı oluştur
            var managerUser = await userManager.FindByNameAsync("manager@optiviera.com");
            if (managerUser == null)
            {
                managerUser = new WaveUser
                {
                    UserName = "manager@optiviera.com",
                    Email = "manager@optiviera.com",
                    FirstName = "Manager",
                    LastName = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(managerUser, "Manager123!");
                await userManager.AddToRoleAsync(managerUser, "Manager");
            }

            // Employee kullanıcısı oluştur
            var employeeUser = await userManager.FindByNameAsync("employee@optiviera.com");
            if (employeeUser == null)
            {
                employeeUser = new WaveUser
                {
                    UserName = "employee@optiviera.com",
                    Email = "employee@optiviera.com",
                    FirstName = "Employee",
                    LastName = "User",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(employeeUser, "Employee123!");
                await userManager.AddToRoleAsync(employeeUser, "Employee");
            }

            // Priority'leri oluştur
            if (!context.Priorities.Any())
            {
                var priorities = new[]
                {
                    new Priority { Name = "Düşük" },
                    new Priority { Name = "Orta" },
                    new Priority { Name = "Yüksek" },
                    new Priority { Name = "Kritik" }
                };

                context.Priorities.AddRange(priorities);
                await context.SaveChangesAsync();
            }
        }
    }
}
