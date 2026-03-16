using HotelApi.Data;
using HotelApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoomsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var rooms = await _db.Rooms
            .Include(r => r.Reservation)
            .Select(r => new
            {
                r.Id,
                r.RoomNumber,
                r.Type,
                r.PricePerNight,
                IsAvailable = r.Reservation == null
            })
            .ToListAsync();

        return Ok(rooms);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var room = await _db.Rooms
            .Include(r => r.Reservation)
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.RoomNumber,
                r.Type,
                r.PricePerNight,
                IsAvailable = r.Reservation == null,
                CurrentReservation = r.Reservation == null ? null : new
                {
                    r.Reservation.Id,
                    r.Reservation.GuestName,
                    r.Reservation.GuestEmail,
                    r.Reservation.CheckIn,
                    r.Reservation.CheckOut
                }
            })
            .FirstOrDefaultAsync();

        if (room == null)
            return NotFound(new { message = $"Room with id {id} not found." });

        return Ok(room);
    }
}