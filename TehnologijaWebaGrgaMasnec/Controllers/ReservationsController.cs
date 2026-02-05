using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Infrastructure;
using TehnologijaWebaGrgaMasnec.Entities;
using static TehnologijaWebaGrgaMasnec.Entities.Reservation;
using TehnologijaWebaGrgaMasnec.DTos;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ReservationsController(AppDbContext db) => _db = db;

    // User rezervira termin
    [HttpPost]
    [Authorize] // User ili Admin
    public async Task<IActionResult> Create(ReservationCreateDto dto) //POST /api/reservations
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        // postoji li slot?
        var slotExists = await _db.TimeSlots.AnyAsync(t => t.Id == dto.TimeSlotId);
        if (!slotExists) return NotFound("TimeSlot ne postoji.");

        // je li zauzet (samo Active blokira)
        var isTaken = await _db.Reservations.AnyAsync(r =>
            r.TimeSlotId == dto.TimeSlotId && r.Status == Reservation.ReservationStatus.Active);

        if (isTaken) return BadRequest("Termin je već rezerviran.");

        var reservation = new Reservation
        {
            UserId = userId,
            TimeSlotId = dto.TimeSlotId,
            Status = Reservation.ReservationStatus.Active,
            Created = DateTime.Now
        };

        _db.Reservations.Add(reservation); //u DB dodaje rezervaciju

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return BadRequest("Termin je upravo rezervirao netko drugi.");
        }

        // rezervacija se sad nalazi u bazi
        return Ok(new
        {
            reservation.Id,
            reservation.TimeSlotId,
            reservation.UserId,
            reservation.Status,
            reservation.Created
        });
    }

    // User vidi svoje rezervacije
    [HttpGet("mine")]
    [Authorize]
    public async Task<IActionResult> Mine() //GET/api/reservations/mine
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var data = await _db.Reservations       // lista rezervacija korisnika
            .Where(r => r.UserId == userId)
            .Include(r => r.TimeSlot)
            .ThenInclude(t => t.Room)
            .Select(r => new
            {
                r.Id,
                r.Status,
                r.Created,
                TimeSlot = new
                {
                    r.TimeSlotId,
                    r.TimeSlot.Start,
                    r.TimeSlot.End,
                    RoomNumber = r.TimeSlot.Room.Number
                }
            })
            .ToListAsync();

        return Ok(data);
    }

    // Cancel (user otkazuje svoju rezervaciju)
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id) // DELETE/api/reservations/{id}
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var r = await _db.Reservations.FirstOrDefaultAsync(x => x.Id == id); // trazi rezervaciju po ID
        if (r == null) return NotFound();

        
        var isAdmin = User.IsInRole("Admin"); // user smije samo svoju rezervaciju, admin može sve
        if (!isAdmin && r.UserId != userId)
            return Forbid();

        r.Status = ReservationStatus.Cancelled;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

