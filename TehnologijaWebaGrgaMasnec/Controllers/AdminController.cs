using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase //GET/api/admin/secret
{
    [HttpGet("secret")]
    [Authorize(Roles = "Admin")]
    public IActionResult Secret() => Ok("Samo admin vidi ovo");
}
