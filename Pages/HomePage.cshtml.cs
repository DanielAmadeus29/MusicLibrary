using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

public class HomePageModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public HomePageModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<UserMusic> Songs { get; set; } = new List<UserMusic>();
    public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    public string Username { get; set; } = "Guest";

    [BindProperty]
    public int SongId { get; set; } // Binds the song ID from the form submission

    [BindProperty]
    public int PlaylistId { get; set; } // Binds the selected playlist ID from the form for adding songs

    [BindProperty]
    public int DeletePlaylistId { get; set; } // Binds the selected playlist ID from the form for deleting playlists

    public async Task OnGetAsync()
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            // Fetch user-specific songs with their associated playlists
            Songs = await _dbContext.UserMusic
                .Include(s => s.Playlist) // Include playlists to show associations
                .Where(song => song.UserId == userId)
                .ToListAsync();

            Username = User.Identity.Name;

            // Fetch all playlists
            Playlists = await _dbContext.Playlist.ToListAsync();
        }
    }

    public async Task<IActionResult> OnPostAddToPlaylistAsync()
    {
        // Include the Playlists collection so EF Core can track changes
        var playlist = await _dbContext.Playlist
            .Include(p => p.Songs)
            .FirstOrDefaultAsync(p => p.Id == PlaylistId);

        // Include Playlists on the song to maintain the many-to-many relationship properly
        var song = await _dbContext.UserMusic
            .Include(s => s.Playlist)
            .FirstOrDefaultAsync(s => s.Id == SongId);

        if (song == null || playlist == null)
        {
            ModelState.AddModelError("", "Invalid song or playlist selection.");
            return Page();
        }

        // Add the song to the playlist if it's not already there
        if (!playlist.Songs.Any(s => s.Id == song.Id))
        {
            playlist.Songs.Add(song);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToPage("/HomePage");
    }

    public async Task<IActionResult> OnPostDeletePlaylistAsync()
    {
        // Fetch the playlist by ID
        var playlist = await _dbContext.Playlist
            .Include(p => p.Songs) // Include Songs to handle relationships properly
            .FirstOrDefaultAsync(p => p.Id == DeletePlaylistId);

        if (playlist != null)
        {
            // Remove the playlist from the database
            _dbContext.Playlist.Remove(playlist);
            await _dbContext.SaveChangesAsync();
        }

        // Redirect back to the same page after deletion
        return RedirectToPage("/HomePage");
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        // Find the song by its ID
        var song = await _dbContext.UserMusic.FindAsync(SongId);
        if (song != null)
        {
            _dbContext.UserMusic.Remove(song); // Remove the song from the database
            await _dbContext.SaveChangesAsync(); // Save changes
        }

        // Redirect back to the same page
        return RedirectToPage("/HomePage");
    }
}
