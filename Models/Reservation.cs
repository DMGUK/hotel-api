namespace HotelApi.Models;

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    
    public Room? Room { get; set; }
}