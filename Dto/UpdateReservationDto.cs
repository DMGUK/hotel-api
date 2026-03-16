namespace HotelApi.Dto;
using System.ComponentModel.DataAnnotations;

public class UpdateReservationDto
{
    [Required]
    [MaxLength(100)]
    public string GuestName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string GuestEmail { get; set; } = string.Empty;

    [Required]
    public DateTime CheckIn { get; set; }

    [Required]
    public DateTime CheckOut { get; set; }
}