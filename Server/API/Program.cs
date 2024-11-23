using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure DbContext with PostgreSQL
        builder.Services.AddDbContext<DBContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Configure Identity to use Guid as the key type
        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DBContext>()
            .AddDefaultTokenProviders(); // Ensures token-based features like email confirmation, password reset, etc.

        // Add Swagger for API documentation
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add Controllers
        builder.Services.AddControllers();

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

        app.UseHttpsRedirection();

        // Enable Authentication and Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map Controllers
        app.MapControllers();

        // Run the application
        app.Run();
    }
}