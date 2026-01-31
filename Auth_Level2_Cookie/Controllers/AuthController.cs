using System.Security.Claims;
using Auth_Level2_Cookie.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Auth_Level2_Cookie.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _db.Users.SingleOrDefault(x => x.Username == username && x.Password == password);
        if (user == null) return Unauthorized("Invalid Credentials");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            // role removed from User model in this project
        };

        var identity = new ClaimsIdentity(claims, "Oreo");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("Oreo", principal);

        return Ok("Logged In");
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Oreo");
        return Ok("Logged Out");
    }

    // Protected Endpoint
    [HttpGet("secure")]
    public IActionResult GetSecure()
    {
        // Check if User is Authenticated
        if (!(User?.Identity?.IsAuthenticated ?? false)) return Unauthorized();

        var name = User?.Identity?.Name ?? "Unknown";
        return Ok($"Hello {name}, you are accessing secure data!");
    }
}
