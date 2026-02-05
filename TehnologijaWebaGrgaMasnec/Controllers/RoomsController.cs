using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Infrastructure;
using TehnologijaWebaGrgaMasnec.Entities;
using TehnologijaWebaGrgaMasnec.DTos;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _db;

    public RoomsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() //GET/api/rooms
    {
        var rooms = await _db.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Facility)
            .Select(r => new
            {
                r.Id,
                r.Number,
                r.Floor,
                Facility = r.Facility.Name,
                RoomType = r.RoomType.Name,
                r.RoomType.Capacity,
                r.RoomType.BasePricePerNight
            })
            .ToListAsync();

        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(RoomCreateDto dto) //POST/api/rooms samo admin
    {
        var facilityExists = await _db.Facilities.AnyAsync(f => f.Id == dto.FacilityId);
        if (!facilityExists) return BadRequest("FacilityId ne postoji.");

        var roomTypeExists = await _db.RoomTypes.AnyAsync(rt => rt.Id == dto.RoomTypeId);
        if (!roomTypeExists) return BadRequest("RoomTypeId ne postoji.");

        var room = new Room
        {
            FacilityId = dto.FacilityId,
            RoomTypeId = dto.RoomTypeId,
            Number = dto.Number,
            Floor = dto.Floor
        };

        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = room.Id }, room);
    }
}

