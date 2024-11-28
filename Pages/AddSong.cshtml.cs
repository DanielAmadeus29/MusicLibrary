using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.Models;
using System;
using System.IO;
using System.Threading.Tasks;

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
        public string Title { get; set; }

        [BindProperty]
        public string Artist { get; set; }

        [BindProperty]
        public IFormFile MusicFile { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Validate user
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    Message = "You must be logged in to add a song.";
                    return Page();
                }

                // Validate file
                if (MusicFile == null || MusicFile.Length == 0)
                {
                    Message = "Please select a valid music file.";
                    return Page();
                }

                // Generate unique filename and save file to "wwwroot/music"
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(MusicFile.FileName)}";
                var filePath = Path.Combine("wwwroot", "music", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(filePath)); // Ensure directory exists

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await MusicFile.CopyToAsync(stream);
                }

                // Save metadata to database
                var music = new UserMusic
                {
                    UserId = int.Parse(userId),
                    Title = Title,
                    Artist = Artist,
                    FilePath = $"/music/{fileName}"
                };

                _dbContext.UserMusic.Add(music); // This should now work
                await _dbContext.SaveChangesAsync();


                Message = "Song added successfully!";
            }
            catch (Exception ex)
            {
                Message = "An error occurred while adding the song.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Page();
        }
    }
}
