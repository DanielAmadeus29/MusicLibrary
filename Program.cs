using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Configure DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MusicLibraryDatabase")));

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Redirect if not authenticated
        options.LogoutPath = "/Logout"; // Path for logging out
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/Logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Clear authentication cookies
    return Results.Redirect("/Login"); // Redirect to login page
});

// Error page for development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Enable cookie-based authentication
app.UseAuthorization(); // Enable authorization

app.MapRazorPages();

app.Run();
