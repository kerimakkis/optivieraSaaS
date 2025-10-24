using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Optiviera.Models;

namespace Optiviera.Data
{
    public class ApplicationDbContext : IdentityDbContext<WaveUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Multi-Tenant
        public DbSet<Tenant> Tenants { get; set; }

        // Core Entities
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Priority> Priorities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Tenant relationships
            builder.Entity<Tenant>()
                .HasMany(t => t.Users)
                .WithOne(u => u.Tenant)
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tenant>()
                .HasMany(t => t.Tickets)
                .WithOne(tk => tk.Tenant)
                .HasForeignKey(tk => tk.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tenant>()
                .HasMany(t => t.Priorities)
                .WithOne(p => p.Tenant)
                .HasForeignKey(p => p.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Ticket relationships
            builder.Entity<Ticket>()
                .HasOne(t => t.Priority)
                .WithMany(p => p.Tickets)
                .HasForeignKey(t => t.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Ticket>()
                .HasMany(t => t.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.Entity<Ticket>()
                .HasIndex(t => t.TenantId);

            builder.Entity<Comment>()
                .HasIndex(c => c.TenantId);

            builder.Entity<Priority>()
                .HasIndex(p => p.TenantId);

            builder.Entity<WaveUser>()
                .HasIndex(u => u.TenantId);
        }
    }
}