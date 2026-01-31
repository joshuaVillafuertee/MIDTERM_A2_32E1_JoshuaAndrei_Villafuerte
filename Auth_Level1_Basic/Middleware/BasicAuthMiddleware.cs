using System.Net.Http.Headers;
using System.Text;
using Auth_Level1_Basic.Data;
using Auth_Level1_Basic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Auth_Level1_Basic.Middleware;


public class SomeRandomMiddleware
{
    private readonly RequestDelegate _next;

    public SomeRandomMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
       
        var temp = context.Request.Headers;
        Console.WriteLine("Hello from SomeRandomMiddleware");
        await _next(context);
    }
}


public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)//, AppDbContext db)
    {
        // If no Authorization header found, just return 401
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues) || StringValues.IsNullOrEmpty(authHeaderValues))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var headerString = authHeaderValues.ToString();
        if (!AuthenticationHeaderValue.TryParse(headerString, out var authHeader))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var parameter = authHeader.Parameter;
        if (string.IsNullOrEmpty(parameter))
        {
            context.Response.StatusCode = 401;
            return;
        }

        byte[] credentialBytes;
        try
        {
            credentialBytes = Convert.FromBase64String(parameter);
        }
        catch
        {
            context.Response.StatusCode = 401;
            return;
        }

        var credentialString = Encoding.UTF8.GetString(credentialBytes);
        var credentials = credentialString.Split(':', 2);
        if (credentials.Length < 2)
        {
            context.Response.StatusCode = 401;
            return;
        }

        var username = credentials[0];
        var password = credentials[1];

        // Verify User (placeholder - use DB in real app)
        User? user = null; //= await db.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (username == "admin" && password == "123")
        {
            user = new User { Username = "admin", Password = "123" };
        }

        if (user == null)
        {
            context.Response.StatusCode = 401;
            return;
        }

        // Success: attach user
        context.Items["User"] = user;

        await _next(context);
    }
}
