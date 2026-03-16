using HotelApi.Data;
using HotelApi.Dto;
using HotelApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var reservation = await _db.Reservations
            .Include(r => r.Room)
            .Where(r => r.Id == id)
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
            .FirstOrDefaultAsync();

        if (reservation == null)
            return NotFound(new { message = $"Reservation with id {id} not found." });

        return Ok(reservation);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create([FromBody] CreateReservationDto dto)
    {
        var room = await _db.Rooms.FindAsync(dto.RoomId);
        if (room == null)
            return NotFound(new { message = $"Room with id {dto.RoomId} not found." });

        bool hasOverlap = await _db.Reservations
            .AnyAsync(r => r.RoomId == dto.RoomId &&
                        r.CheckIn < dto.CheckOut &&
                        r.CheckOut > dto.CheckIn);

        if (hasOverlap)
            return Conflict(new { message = $"Room {room.RoomNumber} is already reserved for the selected dates." });

        if (dto.CheckOut <= dto.CheckIn)
            return BadRequest(new { message = "Check-out must be after check-in." });

        var reservation = new Reservation
        {
            RoomId     = dto.RoomId,
            GuestName  = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            CheckIn    = dto.CheckIn,
            CheckOut   = dto.CheckOut
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, new
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

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateReservationDto dto)
    {
        var reservation = await _db.Reservations.FindAsync(id);


        if (reservation == null)
            return NotFound(new { message = $"Reservation with id {id} not found." });

        bool hasOverlap = await _db.Reservations
            .AnyAsync(r => r.RoomId == reservation.RoomId &&
                        r.Id != id &&
                        r.CheckIn < dto.CheckOut &&
                        r.CheckOut > dto.CheckIn);

        if (hasOverlap)
            return Conflict(new { message = "Room is already reserved for the selected dates." });

        if (dto.CheckOut <= dto.CheckIn)
            return BadRequest(new { message = "Check-out must be after check-in." });

        reservation.GuestName  = dto.GuestName;
        reservation.GuestEmail = dto.GuestEmail;
        reservation.CheckIn    = dto.CheckIn;
        reservation.CheckOut   = dto.CheckOut;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            reservation.Id,
            reservation.GuestName,
            reservation.GuestEmail,
            reservation.CheckIn,
            reservation.CheckOut,
            reservation.RoomId
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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