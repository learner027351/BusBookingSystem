
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;


namespace BusBooking.Infrastructure.Data
{
    public class BusBookDbContextFactory : IDesignTimeDbContextFactory<BusBookDbContext>
    {
        public BusBookDbContext CreateDbContext(string[] args)
        {
            // Build configuration (read from API project)
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../BusBooking.API", "appsettings.json"))
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BusBookDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new BusBookDbContext(optionsBuilder.Options);
        }
    }
}