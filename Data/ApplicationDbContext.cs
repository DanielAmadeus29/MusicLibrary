using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models; // Ensure the namespace for UserMusic is included

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } // Existing Users table
    public DbSet<UserMusic> UserMusic { get; set; } // Add this line for UserMusic
}
