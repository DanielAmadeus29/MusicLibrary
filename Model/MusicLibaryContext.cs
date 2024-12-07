using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models; // Add this

public class MusicLibraryContext : DbContext
{
    public MusicLibraryContext(DbContextOptions<MusicLibraryContext> options)
        : base(options)
    {
    }

    public DbSet<UserMusic> UserMusics { get; set; }
}
