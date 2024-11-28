using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.Models;

namespace MusicLibrary.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public RegisterModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }

        [BindProperty]
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            // Display the form
        }

        public IActionResult OnPost()
        {
            // Validate form inputs
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ErrorMessage = "All fields are required.";
                return Page();
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return Page();
            }

            // Check if username already exists
            var existingUser = _dbContext.Users.FirstOrDefault(u => u.Username == Username);
            if (existingUser != null)
            {
                ErrorMessage = "Username is already taken.";
                return Page();
            }

            // Insert new user into database
            var newUser = new User
            {
                Username = Username,
                Password = Password // Store as plain text (not secure; use hashing in production)
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            SuccessMessage = "Registration successful! You can now log in.";
            return Page();
        }
    }
}
