using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.Models;
using System.Security.Claims;

namespace MusicLibrary.Pages
{
    public class AddSongModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public AddSongModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        public string Artist { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile MusicFile { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                ErrorMessage = "You must be logged in to add a song.";
                return Page();
            }

            if (string.IsNullOrEmpty(Title) || string.IsNullOrEmpty(Artist) || MusicFile == null)
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            // Collect UserID
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            // Save File
            var filePath = Path.Combine("wwwroot/music", MusicFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await MusicFile.CopyToAsync(stream);
            }

            // Create Save Music
            var userMusic = new UserMusic
            {
                Title = Title,
                Artist = Artist,
                FilePath = $"/music/{MusicFile.FileName}",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.UserMusic.Add(userMusic);
            await _dbContext.SaveChangesAsync();

        
            return RedirectToPage("/HomePage");
        }
    }
}
