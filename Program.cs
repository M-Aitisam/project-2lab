using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project_2lab.Areas.Identity;
using project_2lab.Data;
using project_2lab.Pages;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
//builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<WeatherForecastService>();

// Add default identity and roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();  // Add SignalR services
builder.Services.AddSingleton<AnnouncementService>();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

// Authorization for roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Manager", policy => policy.RequireRole("Manager"));
});

var app = builder.Build();


// Ensure roles and default admin user are created
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    try
    {
        var roles = new[] { "Manager", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminEmail = "aitisam@gmail.com";
        var adminPassword = "Aitisam@1234";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = "Aitisam", Email = adminEmail, EmailConfirmed = true };
            var createResult = await userManager.CreateAsync(adminUser, adminPassword);

            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Manager");
            }
            else
            {
                Console.WriteLine("Admin user creation failed: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            if (!await userManager.IsInRoleAsync(adminUser, "Manager"))
            {
                await userManager.AddToRoleAsync(adminUser, "Manager");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error ensuring roles and admin user: {ex.Message}");
    }
}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();  // Map the Blazor Hub
app.MapFallbackToPage("/_Host");

app.Run();
