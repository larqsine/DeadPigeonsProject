using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service;
using Service.Interfaces;
using Service.Security;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<DBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity to use Guid as the key type
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.User.RequireUniqueEmail = true; // Ensures unique emails
    })
    .AddEntityFrameworkStores<DBContext>()
    .AddDefaultTokenProviders()
    .AddUserValidator<CustomEmailValidator<User>>(); // Use a custom validator
    ; // Ensures token-based features like email confirmation, password reset, etc.
builder.Services.AddScoped<IPasswordHasher<User>, Argon2idPasswordHasher<User>>();

builder.Services.AddScoped<UserRepository>();


// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers
builder.Services.AddControllers();

// Register repositories
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<BoardRepository>();

// Register services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddTransient<WeeklyGameJob>();



var app = builder.Build();

// Create roles if they don't exist
CreateRoles(app);

// Configure Swagger for Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at the root
    });
}

app.UseHttpsRedirection();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Run the application
app.Run();

// Method to create roles if they don't exist
void CreateRoles(IApplicationBuilder app)
{
    var scope = app.ApplicationServices.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // List of roles to create
    var roles = new[] { "Admin", "Player" };

    foreach (var role in roles)
    {
        // Check if the role exists, if not, create it
        var roleExists = roleManager.RoleExistsAsync(role).Result;
        if (!roleExists)
        {
            var identityRole = new IdentityRole<Guid>(role);
            roleManager.CreateAsync(identityRole).Wait();
        }
    }

    // Optionally create an Admin user
    var admin = userManager.FindByEmailAsync("admin@example.com").Result;
    if (admin == null)
    {
        var user = new User { UserName = "admin", Email = "admin@example.com" };
        var result = userManager.CreateAsync(user, "AdminPassword123").Result;
        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(user, "Admin").Wait();
        }
    }
}
