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
        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, RoomNumber = "101", Type = "Single", PricePerNight = 80 },
            new Room { Id = 2, RoomNumber = "102", Type = "Single", PricePerNight = 90 },
            new Room { Id = 3, RoomNumber = "103", Type = "Double", PricePerNight = 120 },
            new Room { Id = 4, RoomNumber = "201", Type = "Double", PricePerNight = 70 },
            new Room { Id = 5, RoomNumber = "202", Type = "Suite",  PricePerNight = 150 },
            new Room { Id = 6, RoomNumber = "203", Type = "Single", PricePerNight = 95 },
            new Room { Id = 7, RoomNumber = "301", Type = "Single", PricePerNight = 100 },
            new Room { Id = 8, RoomNumber = "302", Type = "Double", PricePerNight = 110 },
            new Room { Id = 9, RoomNumber = "303", Type = "Double", PricePerNight = 140 },
            new Room { Id = 10, RoomNumber = "310", Type = "Suite",  PricePerNight = 250 }
        );
    }
}