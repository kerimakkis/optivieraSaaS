using Microsoft.AspNetCore.Identity;
using Optiviera.Models;
using System.Security.Cryptography;
using System.Text;

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
                    new Priority { Name = "Acil" }
                };

                context.Priorities.AddRange(priorities);
                await context.SaveChangesAsync();
            }

            // Trial lisans oluştur (eğer yoksa)
            if (!context.Licenses.Any())
            {
                var machineId = GenerateMachineId();
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
                await context.SaveChangesAsync();
            }
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
