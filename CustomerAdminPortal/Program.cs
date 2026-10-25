
using CustomerAdminPortal.Data;
using CustomerAdminPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

//MVC Views aur API controllers
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddOpenApi();

//Existing Customers database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(
     builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//Separate Identity database context
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ));

//Login and password settings
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

//Login page Location
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    var adminEmail = builder.Configuration["AdminAccount:Email"];
    var adminPassword = builder.Configuration["AdminAccount:Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) ||
        string.IsNullOrWhiteSpace(adminPassword))
    {
        throw new InvalidOperationException(
            "AdminAccount Email aur Password configuration mein set karein.");
    }

    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ",
                result.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                "Admin account create nahi hua: " + errors);
        }
    }
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

//Authentication must be before Authorization
app.UseAuthentication();
app.UseAuthorization();

//Existing Api routes
app.MapControllers();

//Existing MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");
app.Run();
























