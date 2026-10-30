
using AuthDemo.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Configure cookie authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
    });

var app = builder.Build();
// Create the data folder and seed the admin user
var dataFolder = Path.Combine(app.Environment.ContentRootPath, "data");
Directory.CreateDirectory(dataFolder);

var usersFile = Path.Combine(dataFolder, "users.json");

if (!File.Exists(usersFile) ||
    new FileInfo(usersFile).Length == 0 ||
    JsonSerializer.Deserialize<List<User>>(File.ReadAllText(usersFile))?.Count == 0)
{
    var admin = new User
    {
        Username = "admin"
    };

    var passwordHasher = new PasswordHasher<User>();
    admin.PasswordHash = passwordHasher.HashPassword(admin, "Password123");

    var users = new List<User> { admin };

    File.WriteAllText(
        usersFile,
        JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true })
    );
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication must come before authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
