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

            // Create and save the playlist
            var playlist = new Playlist
            {
                Name = PlaylistName
            };

            _dbContext.Playlist.Add(playlist);
            await _dbContext.SaveChangesAsync();

            // Redirect to the homepage after successful creation
            return RedirectToPage("/HomePage");
        }
    }
}
