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
        public IFormFile? MusicFile { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {

        }

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


            if (MusicFile.Length > 50 * 1024 * 1024)
            {
                ErrorMessage = "File size must be less than 50MB.";
                return Page();
            }


            var allowedExtensions = new[] { ".mp3", ".wav", ".flac", ".ogg", ".aac", ".m4a", ".wma" };
            var fileExtension = Path.GetExtension(MusicFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ErrorMessage = "Only audio files (MP3, WAV, FLAC, OGG, AAC, M4A) are allowed.";
                return Page();
            }

            try
            {
              
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    ErrorMessage = "User authentication error.";
                    return Page();
                }
                var userId = int.Parse(userIdClaim.Value);

                var musicFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "music");
                if (!Directory.Exists(musicFolder))
                {
                    Directory.CreateDirectory(musicFolder);
                }

          
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(musicFolder, uniqueFileName);

     
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await MusicFile.CopyToAsync(stream);
                }

      
                var userMusic = new UserMusic
                {
                    Title = Title,
                    Artist = Artist,
                    FilePath = $"/music/{uniqueFileName}",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.UserMusic.Add(userMusic);
                await _dbContext.SaveChangesAsync();

                return RedirectToPage("/HomePage");
            }
            catch (Exception ex)
            {
     
                ErrorMessage = $"An error occurred while uploading the song: {ex.Message}";
                return Page();
            }
        }
    }
}