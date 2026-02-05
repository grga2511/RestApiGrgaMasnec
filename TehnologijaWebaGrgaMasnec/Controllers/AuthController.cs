using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TehnologijaWebaGrgaMasnec.Entities;

namespace TehnologijaWebaGrgaMasnec.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    [HttpPost("register")] //POST /api/auth/register
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        var user = new ApplicationUser // kreira objekt
        {
            UserName = req.Email,
            Email = req.Email,
            FullName = req.FullName
        };

        var result = await _userManager.CreateAsync(user, req.Password); // sprema u bazu
        if (!result.Succeeded)
            return BadRequest(result.Errors); //provjera 

        await _userManager.AddToRoleAsync(user, "User"); 

        return Ok("Registered"); 
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req) //POST /api/auth/login
    {
        var user = await _userManager.FindByEmailAsync(req.Email); // trazi usera u bazi
        if (user == null) return Unauthorized("Invalid credentials");

        var ok = await _userManager.CheckPasswordAsync(user, req.Password); //uzima lozinku iz baze i usporeduje ju
        if (!ok) return Unauthorized("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user); // dohvati Admin ili User 
        var token = CreateToken(user, roles);

        return Ok(new { token }); // klijent dobije JWT
    }

    private string CreateToken(ApplicationUser user, IList<string> roles)
    {
        var jwt = _config.GetSection("Jwt"); 

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>    // sadrzaj tokena
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        foreach (var role in roles) //dodavanje rola 
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(
                int.Parse(jwt["ExpiresMinutes"]!)
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // iz objekta u tekst
    }
}


// ogranicavaju sto se moze poslat
public record RegisterRequest(string Email, string Password, string? FullName); 
public record LoginRequest(string Email, string Password);

