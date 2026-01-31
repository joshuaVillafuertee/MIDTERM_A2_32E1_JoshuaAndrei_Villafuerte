using System.Text;
using Auth_Level2_Cookie.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Auth_Level2_Cookie.Middleware;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;

    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, AppDbContext db)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authorization header missing");
            return;
        }

        var header = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(header))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Authorization header empty");
            return;
        }

        try
        {
            var encoded = header.Replace("Basic ", "");
            var bytes = Convert.FromBase64String(encoded);
            var decoded = Encoding.UTF8.GetString(bytes);

            var parts = decoded.Split(':');
            var username = parts[0];
            var password = parts[1];

            var user = db.Users
                .FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid credentials");
                return;
            }

            await _next(context);
        }
        catch
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid Authorization header");
        }
    }
}
