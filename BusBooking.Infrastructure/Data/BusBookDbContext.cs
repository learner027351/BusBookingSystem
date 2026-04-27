
using Microsoft.EntityFrameworkCore;
using BusBooking.Core.Entities;

namespace BusBooking.Infrastructure.Data
{
    public class BusBookDbContext:DbContext
    {
        public BusBookDbContext(DbContextOptions<BusBookDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.BusId, b.SeatNumber })
                .IsUnique()
                .HasFilter("[Status] = 'Confirmed'");

            modelBuilder.Entity<Bus>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            
        }

        public DbSet<Bus> Buses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Payment> Payments { get; set; }
    }
}
