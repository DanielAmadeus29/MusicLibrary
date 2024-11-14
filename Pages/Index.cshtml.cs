using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.Data;
using MusicLibrary.Models;
using System.Linq;

namespace MusicLibrary.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            try
            {
                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    ErrorMessage = "Username and Password are required.";
                    return Page();
                }

                var user = _dbContext.Users.FirstOrDefault(u => u.Username == Username && u.Password == Password);

                if (user != null)
                {
                    return RedirectToPage("/HomePage");
                }

                ErrorMessage = "Invalid username or password.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred while processing your request.";
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Page();
        }
    }
}


