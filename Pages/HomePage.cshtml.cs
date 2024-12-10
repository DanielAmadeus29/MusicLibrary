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


    public async Task OnGetAsync()
    {
        // Check UserID
        if (User.Identity.IsAuthenticated)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);

            // Songs Based on User
            Songs = await _dbContext.UserMusic
                .Include(s => s.Playlist) 
                .Where(song => song.UserId == userId) 
                .ToListAsync();

            Username = User.Identity.Name;

            // Song based on Playlist
            Playlists = await _dbContext.Playlist
                .Where(p => p.UserId == userId) 
                .ToListAsync();
        }
        else
        {
         
            Songs = new List<UserMusic>();
            Playlists = new List<Playlist>();
        }
    }
    public async Task<IActionResult> OnPostAddToPlaylistAsync()
    {
        var song = await _dbContext.UserMusic.FirstOrDefaultAsync(s => s.Id == SongId);

        if (song == null)
        {
            ModelState.AddModelError("", "Invalid song selection.");
            return Page();
        }

        if (PlaylistId == null)
        {
    
            song.PlaylistId = null;
            song.Playlist = null; 
            _dbContext.Entry(song).Property(s => s.PlaylistId).IsModified = true; 
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            song.PlaylistId = PlaylistId;
            await _dbContext.SaveChangesAsync();
        }

        return RedirectToPage("/HomePage");
    }



    public async Task<IActionResult> OnPostDeletePlaylistAsync()
    {
  
        var playlist = await _dbContext.Playlist
            .Include(p => p.Songs) 
            .FirstOrDefaultAsync(p => p.Id == DeletePlaylistId);

        if (playlist != null)
        {
   
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
