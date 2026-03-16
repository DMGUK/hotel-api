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
        var now = DateTime.UtcNow;

        var rooms = await _db.Rooms
            .Include(r => r.Reservations)
            .Select(r => new
            {
                r.Id,
                r.RoomNumber,
                r.Type,
                r.PricePerNight,
                IsAvailable = !r.Reservations.Any(res =>
                    res.CheckIn <= now && res.CheckOut >= now)
            })
            .ToListAsync();

        return Ok(rooms);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var now = DateTime.UtcNow;

        var room = await _db.Rooms
            .Include(r => r.Reservations)
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.RoomNumber,
                r.Type,
                r.PricePerNight,
                IsAvailable = !r.Reservations.Any(res =>
                    res.CheckIn <= now && res.CheckOut >= now),
                Reservations = r.Reservations.Select(res => new
                {
                    res.Id,
                    res.GuestName,
                    res.GuestEmail,
                    res.CheckIn,
                    res.CheckOut
                })
            })
            .FirstOrDefaultAsync();

        if (room == null)
            return NotFound(new { message = $"Room with id {id} not found." });

        return Ok(room);
    }
}