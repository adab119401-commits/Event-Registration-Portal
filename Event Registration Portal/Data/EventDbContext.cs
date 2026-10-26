using Microsoft.EntityFrameworkCore;
using Event_Registration_Portal.Models;

namespace Event_Registration_Portal.Data
{
    public class EventDbContext : DbContext
    {
        public EventDbContext(DbContextOptions<EventDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<WaitlistEntry> WaitlistEntries { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasIndex(b => new { b.EventId, b.SeatNumber })
                .IsUnique();
            modelBuilder.Entity<Booking>()
                 .HasIndex(b => new { b.UserId, b.EventId })
                .IsUnique();
            modelBuilder.Entity<WaitlistEntry>()
                .HasIndex(w => new { w.UserId, w.EventId })
                .IsUnique();
        }
    }
}
