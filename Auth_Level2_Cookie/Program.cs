using Auth_Level2_Cookie.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Standard Swagger
// DB for this project
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

builder.Services.AddAuthentication("Oreo")
    .AddCookie("Oreo", options =>
    {
        options.Cookie.Name = "CookieSession";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// register basic auth middleware
app.UseMiddleware<Auth_Level2_Cookie.Middleware.BasicAuthMiddleware>();

app.MapControllers();

// Start the app, open browser to Swagger UI, then wait for shutdown
await app.StartAsync();
try
{
    var server = app.Services.GetRequiredService<IServer>();
    var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
    var first = addresses?.FirstOrDefault() ?? "http://localhost:5000";
    var url = first.TrimEnd('/') + "/swagger";
    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
}
catch
{
    // ignore
}

await app.WaitForShutdownAsync();
