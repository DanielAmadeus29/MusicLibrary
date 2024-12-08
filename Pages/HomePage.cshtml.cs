using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models;
using System.Security.Claims;

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
        // Check if the user is authenticated
        if (User.Identity.IsAuthenticated)
        {
            // Get the user ID from the claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Retrieve the user's songs and username from the database
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                Songs = await _dbContext.UserMusic
                    .Where(song => song.UserId == user.Id)
                    .ToListAsync();

                Username = user.Username;
            }
        }
    }
}
