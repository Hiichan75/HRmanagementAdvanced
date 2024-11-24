using HRmanagementAdvanced.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PersonenDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation
    options.User.RequireUniqueEmail = false;        // Disable unique email constraint
    options.Password.RequireDigit = false; // Disable digit requirement
    options.Password.RequiredLength = 6;  // Set minimum length to 6
    options.Password.RequireNonAlphanumeric = false; // Disable non-alphanumeric requirement
    options.Password.RequireUppercase = false; // Disable uppercase requirement
    options.Password.RequireLowercase = false; // Disable lowercase requirement
})
.AddRoles<IdentityRole>() // Register RoleManager for roles
.AddEntityFrameworkStores<PersonenDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Seed users and roles
    await DatabaseSeeder.SeedUsersAsync(services);
    await DatabaseSeeder.SeedDataAsync(services);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Ensure authentication is included
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employees}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
