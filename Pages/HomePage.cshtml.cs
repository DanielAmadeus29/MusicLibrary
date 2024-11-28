using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MusicLibrary.Pages
{
    [Authorize]
    public class HomePageModel : PageModel
    {
        public string Username { get; private set; }
        public string DebugInfo { get; private set; }

        public void OnGet()
        {
            // Debug: List all claims
            DebugInfo = string.Join(", ", User.Claims.Select(c => $"{c.Type}: {c.Value}"));

            // Retrieve Username from claims
            Username = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown User"
                : "Guest";
        }
    }
}
