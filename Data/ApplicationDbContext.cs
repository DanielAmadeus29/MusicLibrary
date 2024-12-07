using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserMusic> UserMusic { get; set; }
    public DbSet<User> Users { get; set; }
}
