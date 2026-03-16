using HotelApi.Data;
using HotelApi.Models;
using HotelApi.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReservationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var reservations = await _db.Reservations
            .Include(r => r.Room)
            .Select(r => new
            {
                r.Id,
                r.GuestName,
                r.GuestEmail,
                r.CheckIn,
                r.CheckOut,
                Room = new
                {
                    r.Room!.Id,
                    r.Room.RoomNumber,
                    r.Room.Type,
                    r.Room.PricePerNight
                }
            })
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var room = await _db.Rooms.FindAsync(dto.RoomId);
        if (room == null)
            return NotFound(new { message = $"Room with id {dto.RoomId} not found." });

        bool alreadyReserved = await _db.Reservations
            .AnyAsync(r => r.RoomId == dto.RoomId);

        if (alreadyReserved)
            return Conflict(new { message = $"Room {room.RoomNumber} is already reserved." });

        if (dto.CheckOut <= dto.CheckIn)
            return BadRequest(new { message = "Check-out must be after check-in." });

        var reservation = new Reservation
        {
            RoomId    = dto.RoomId,
            GuestName  = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            CheckIn    = dto.CheckIn,
            CheckOut   = dto.CheckOut
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = reservation.Id }, new
        {
            reservation.Id,
            reservation.GuestName,
            reservation.GuestEmail,
            reservation.CheckIn,
            reservation.CheckOut,
            reservation.RoomId,
            RoomNumber = room.RoomNumber
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var reservation = await _db.Reservations.FindAsync(id);

        if (reservation == null)
            return NotFound(new { message = $"Reservation with id {id} not found." });

        _db.Reservations.Remove(reservation);
        await _db.SaveChangesAsync();

        return Ok(new { message = $"Reservation {id} cancelled successfully." });
    }
}