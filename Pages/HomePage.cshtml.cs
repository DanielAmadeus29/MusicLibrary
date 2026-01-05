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
    public int SongId { get; set; }

    [BindProperty]
    public int? PlaylistId { get; set; }

    [BindProperty]
    public int DeletePlaylistId { get; set; }

    [BindProperty]
    public string PlaylistName { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = int.Parse(userIdClaim.Value);

                Songs = await _dbContext.UserMusic
                    .Include(s => s.Playlist)
                    .Where(song => song.UserId == userId)
                    .ToListAsync();

                Username = User.Identity.Name ?? "Guest";

                Playlists = await _dbContext.Playlist
                    .Include(p => p.Songs)
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }
        }
        else
        {
            Songs = new List<UserMusic>();
            Playlists = new List<Playlist>();
        }
    }

    public async Task<IActionResult> OnPostCreatePlaylistAsync()
    {
        if (string.IsNullOrEmpty(PlaylistName))
        {
            ModelState.AddModelError("", "Playlist name is required.");
            await OnGetAsync();
            return Page();
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return RedirectToPage("/Index");
        }

        var userId = int.Parse(userIdClaim.Value);

        var playlist = new Playlist
        {
            Name = PlaylistName,
            UserId = userId
        };

        _dbContext.Playlist.Add(playlist);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Error saving playlist: {ex.Message}");
            await OnGetAsync();
            return Page();
        }

        return RedirectToPage("/HomePage");
    }

    public async Task<IActionResult> OnPostAddToPlaylistAsync()
    {
        var song = await _dbContext.UserMusic.FirstOrDefaultAsync(s => s.Id == SongId);
        if (song == null)
        {
            ModelState.AddModelError("", "Invalid song selection.");
            await OnGetAsync();
            return Page();
        }

        // If PlaylistId is null, 0, or empty, remove from playlist
        if (!PlaylistId.HasValue || PlaylistId.Value == 0)
        {
            song.PlaylistId = null;
            song.Playlist = null;
        }
        else
        {
            song.PlaylistId = PlaylistId.Value;
        }

        await _dbContext.SaveChangesAsync();
        return RedirectToPage("/HomePage");
    }

    public async Task<IActionResult> OnPostDeletePlaylistAsync()
    {
        var playlist = await _dbContext.Playlist
            .Include(p => p.Songs)
            .FirstOrDefaultAsync(p => p.Id == DeletePlaylistId);

        if (playlist != null)
        {
            // Remove playlist reference from all songs
            if (playlist.Songs != null)
            {
                foreach (var song in playlist.Songs)
                {
                    song.PlaylistId = null;
                }
            }

            _dbContext.Playlist.Remove(playlist);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToPage("/HomePage");
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var song = await _dbContext.UserMusic.FindAsync(SongId);
        if (song != null)
        {
            _dbContext.UserMusic.Remove(song);
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToPage("/HomePage");
    }
}