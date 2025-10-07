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
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<LicenseHistory> LicenseHistories { get; set; }
    }
}