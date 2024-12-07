using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.Models;
using Microsoft.EntityFrameworkCore;

public class HomePageModel : PageModel
{
    private readonly ApplicationDbContext _dbContext;

    public HomePageModel(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<UserMusic> Songs { get; set; } = new List<UserMusic>();
    public string Username { get; set; } = "Guest";

    public async Task OnGetAsync()
    {
        int userId = 2; // Replace with logic to fetch the logged-in user's ID
        Songs = await _dbContext.UserMusic
            .Where(song => song.UserId == userId)
            .ToListAsync();

        var user = await _dbContext.Users.FindAsync(userId);
        Username = user?.Username ?? "Guest";
    }
}
