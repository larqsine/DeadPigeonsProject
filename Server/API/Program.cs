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
builder.Services.AddScoped<IPasswordHasher<User>, Argon2idPasswordHasher<User>>();

// Register Repositories and Services
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<BoardRepository>();

// Register services
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddTransient<WeeklyGameJob>();
builder.Services.AddScoped<TransactionRepository>();
builder.Services.AddScoped<TransactionService>();

// Add Controllers
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin() // Allow any origin
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


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

app.UseCors("AllowAll");

// Enable Authentication and Authorization
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Run the application
app.Run();


