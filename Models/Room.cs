namespace HotelApi.Models;

public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }

    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}