using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Optiviera.Data;
using Optiviera.Models;
using Optiviera.Services;
using Optiviera.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Determine database path based on environment
string dbPath;
var isElectronMode = Environment.GetEnvironmentVariable("ELECTRON_MODE") == "true";

if (isElectronMode)
{
    // In Electron/Production: Store database in user's AppData folder
    var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    var appFolder = Path.Combine(appDataPath, "Optiviera ERP");
    Directory.CreateDirectory(appFolder);
    dbPath = Path.Combine(appFolder, "Optiviera.db");
    Console.WriteLine($"Using database path: {dbPath}");
}
else
{
    // In Development: Use relative path
    dbPath = "Optiviera.db";
}

// Add services to the container.
var connectionString = $"Data Source={dbPath}";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<WaveUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders();

// Add localization services
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

// SQLite doesn't need this switch

builder.Services.AddScoped<ITTicketService, TicketService>();

builder.Services.AddScoped<ITRolesService, RolesService>();

// Add license service
builder.Services.AddScoped<ILicenseService, LicenseService>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Auto-migrate database on startup (especially important for Electron mode)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<WaveUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Ensure database is created and migrated
        context.Database.Migrate();
        Console.WriteLine("Database migrated successfully.");
        
        // Seed initial data
        SeedData.Initialize(services).Wait();
        Console.WriteLine("Initial data seeded successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during database initialization: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


// Only use HTTPS redirection in non-Electron mode (Electron uses HTTP)
if (!isElectronMode)
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

// Configure request localization
var supportedCultures = new[] { "tr", "en", "de", "fr", "es", "it", "pt", "nl" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
app.UseRequestLocalization(localizationOptions);

app.UseRouting();

// Add session support for license middleware
app.UseSession();

// Add license middleware
app.UseMiddleware<Optiviera.Middleware.LicenseMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Seed data (with error handling)
// Note: Data is already seeded during migration (line 80), but we run it again
// to ensure any updates are applied
try
{
    using (var scope = app.Services.CreateScope())
    {
        await SeedData.Initialize(scope.ServiceProvider);
    }
    Console.WriteLine("Final data seed completed successfully.");
}
catch (Exception ex)
{
    // Don't crash the application if seeding fails on second run
    Console.WriteLine($"Warning: Second data seed failed: {ex.Message}");
    Console.WriteLine($"This is usually not critical as data was already seeded during migration.");
}

Console.WriteLine("Starting web server...");
app.Run();
Console.WriteLine("Application stopped.");
