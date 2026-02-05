using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TehnologijaWebaGrgaMasnec.DTos;
using TehnologijaWebaGrgaMasnec.Entities;
using TehnologijaWebaGrgaMasnec.Repositories;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotRepository _repo;
    public TimeSlotsController(ITimeSlotRepository repo) => _repo = repo;

    
    // Admin kreira termin
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] TimeSlotCreateDto dto) // POST /api/timeslots
    {
        //  Validacija datuma
        if (dto.End <= dto.Start)
            return BadRequest("End mora biti veći od Start.");

        //  Provjera da soba postoji
        var roomExists = await _repo.RoomExistsAsync(dto.RoomId);
        if (!roomExists) return BadRequest("RoomId ne postoji.");

        // Sprjecavanje preklapanja termina
        var overlaps = await _repo.HasOverlapAsync(dto.RoomId, dto.Start, dto.End);
        if (overlaps) return BadRequest("Termin se preklapa s postojećim terminom.");

        
        var slot = new TimeSlot
        {
            RoomId = dto.RoomId,
            Start = dto.Start,
            End = dto.End
        };

        slot = await _repo.AddAsync(slot);

        
        var count = await _repo.CountAsync();

        return Ok(new { slot.Id, CountInDb = count });
    }

    
    // Svi mogu vidjeti slobodne termine
    [HttpGet("available")]
    public async Task<IActionResult> Available([FromQuery] int roomId, [FromQuery] DateTime from, [FromQuery] DateTime to) // GET /api/timeslots/available
    {
        if (from >= to) return BadRequest("from mora biti prije to.");

        var data = await _repo.GetAvailableAsync(roomId, from, to);
        return Ok(data);
    }

    // Svi termini (za pregled) + HasReservation
    // GET /api/timeslots/all
    [HttpGet("all")]
    public async Task<IActionResult> All()
    {
        var data = await _repo.GetAllAsync();
        return Ok(data);
    }

   
}
