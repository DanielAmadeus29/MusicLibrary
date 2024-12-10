using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.Models;

namespace MusicLibrary.Pages
{
    public class CreatePlaylistModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public CreatePlaylistModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string PlaylistName { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {
            // No specific logic needed for Get request
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(PlaylistName))
            {
                ErrorMessage = "Playlist name is required.";
                return Page();
            }

            // Retrieve the UserId of the logged-in user
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                ErrorMessage = "User not logged in.";
                return Page(); // Handle unauthenticated users
            }

            var userId = int.Parse(userIdClaim.Value); // Convert UserId to integer

            // Create and save the Playlist
            var playlist = new Playlist
            {
                Name = PlaylistName,
                UserId = userId // Associate the playlist with the logged-in user
            };

            _dbContext.Playlist.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Redirect to the homepage after successful creation
            return RedirectToPage("/HomePage");
        }

    }
}
