using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models;
using System.Linq;
using System.Threading.Tasks;

public class HomePageModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public HomePageModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<UserMusic> Songs { get; set; } = new List<UserMusic>();
    public string Username { get; set; } = "Guest";

    [BindProperty]
    public int SongId { get; set; } // Binds the song ID from the form submission

    public async Task OnGetAsync()
    {
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            Songs = await _dbContext.UserMusic.Where(song => song.UserId == userId).ToListAsync();
            Username = User.Identity.Name;
        }
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
