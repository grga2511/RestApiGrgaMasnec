using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TehnologijaWebaGrgaMasnec.Infrastructure;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacilitiesController : ControllerBase 
{
    private readonly AppDbContext _db;
    public FacilitiesController(AppDbContext db) => _db = db;

    [HttpGet] //GET /api/facilities
    public async Task<IActionResult> Get()
    {
        var data = await _db.Facilities
            .Select(f => new { f.Id, f.Name, f.Address })
            .ToListAsync();

        return Ok(data);
    }
}

