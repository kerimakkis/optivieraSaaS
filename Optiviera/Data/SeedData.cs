using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            // Rolleri oluştur - optimized: check all roles at once
            string[] roleNames = { "Admin", "Manager", "Employee", "Not Verified" };
            var existingRoles = roleManager.Roles.Select(r => r.Name).ToList();

            foreach (var roleName in roleNames)
            {
                if (!existingRoles.Contains(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Check all test users at once - optimized
            var testUserEmails = new[] { "admin@optiviera.com", "manager@optiviera.com", "employee@optiviera.com" };
            var existingUsers = await context.Users
                .Where(u => testUserEmails.Contains(u.Email))
                .Select(u => u.Email)
                .ToListAsync();

            // Admin kullanıcısı oluştur
            if (!existingUsers.Contains("admin@optiviera.com"))
            {
                var adminUser = new WaveUser
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
            if (!existingUsers.Contains("manager@optiviera.com"))
            {
                var managerUser = new WaveUser
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
            if (!existingUsers.Contains("employee@optiviera.com"))
            {
                var employeeUser = new WaveUser
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
