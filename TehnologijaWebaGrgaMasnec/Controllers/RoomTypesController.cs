using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Infrastructure;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RoomTypesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get() //GET/api/roomtypes
    {
        var data = await _db.RoomTypes
            .Select(rt => new { rt.Id, rt.Name, rt.Capacity, rt.BasePricePerNight })
            .ToListAsync();

        return Ok(data);
    }
}

