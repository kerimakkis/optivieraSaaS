using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Optiviera.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Design-time connection string for migrations
            // This is only used during migration creation, not at runtime
            // Using ServerVersion.Parse instead of AutoDetect to avoid needing a running MySQL server
            var connectionString = "Server=localhost;Port=3306;Database=optiviera_saas;Uid=root;Pwd=password;";

            optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
