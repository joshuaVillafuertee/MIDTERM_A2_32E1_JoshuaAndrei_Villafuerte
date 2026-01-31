using Microsoft.EntityFrameworkCore;
using Auth_Level2_Cookie.Models;

namespace Auth_Level2_Cookie.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
}
