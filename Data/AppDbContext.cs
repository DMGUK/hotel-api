using HotelApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // THE KEY RULE: one reservation per room
        modelBuilder.Entity<Reservation>()
            .HasIndex(r => r.RoomId)
            .IsUnique();

        // Seed some rooms so the database isn't empty
        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, RoomNumber = "101", Type = "Single", PricePerNight = 80 },
            new Room { Id = 2, RoomNumber = "102", Type = "Single", PricePerNight = 80 },
            new Room { Id = 3, RoomNumber = "201", Type = "Double", PricePerNight = 120 },
            new Room { Id = 4, RoomNumber = "202", Type = "Double", PricePerNight = 120 },
            new Room { Id = 5, RoomNumber = "301", Type = "Suite",  PricePerNight = 250 }
        );
    }
}