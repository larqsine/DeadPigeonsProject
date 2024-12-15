using System.Net;
using System.Text;
using API.Scheduler;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Google.Cloud.SecretManager.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Service;
using Service.Interfaces;
using Service.Security;
using Service.Services;


namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Initialize the Secret Manager client
                var client = SecretManagerServiceClient.Create();

                // Define the secret version name (replace 'AppDb' with your actual secret name)
                // 'latest' will fetch the latest version of the secret
                var secretVersionName = new SecretVersionName("oval-proxy-444716-g0", "AppDb", "latest");

                // Access the secret version
                var secretVersion = client.AccessSecretVersion(secretVersionName);

                // Extract the secret payload (the connection string)
                string connectionString = secretVersion.Payload.Data.ToStringUtf8();

                Console.WriteLine("Connection String: " + connectionString);

                // Use the connection string to configure your DbContext
                var builder = WebApplication.CreateBuilder(args);
                builder.Services.AddDbContext<DBContext>(options =>
                {
                    options.UseNpgsql(connectionString);  // Use the connection string from secret manager
                });

                // Continue with the rest of your application setup...
                builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
                {
                    options.User.RequireUniqueEmail = true; // Ensures unique emails
                })
                .AddEntityFrameworkStores<DBContext>()
                .AddDefaultTokenProviders()
                .AddUserValidator<CustomEmailValidator<User>>(); // Use a custom validator
                builder.Services.AddScoped<IPasswordHasher<User>, Argon2idPasswordHasher<User>>();
                builder.Services.AddScoped<IJwtService, JwtService>();

                // Register Repositories and Services
                builder.Services.AddScoped<UserRepository>();
                builder.Services.AddScoped<PlayerRepository>();
                builder.Services.AddScoped<GameRepository>();
                builder.Services.AddScoped<BoardRepository>();
                builder.Services.AddScoped<WinnerRepository>();
                builder.Services.AddScoped<TransactionRepository>();

                // Register services
                builder.Services.AddScoped<IPlayerService, PlayerService>();
                builder.Services.AddScoped<IGameService, GameService>();
                builder.Services.AddScoped<IBoardService, BoardService>();
                builder.Services.AddScoped<TransactionService>();
                builder.Services.AddScoped<IWinnerService, WinnerService>();

                builder.Services.AddQuartzJobs(); // Add Quartz jobs

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

                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "JwtBearer";
                    options.DefaultChallengeScheme = "JwtBearer";
                })
                .AddJwtBearer("JwtBearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
                    };
                });

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminPolicy", policy =>
                        policy.RequireRole("admin"));
                });
                builder.Services.AddLogging();

                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

                    // Add JWT Authorization
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "Enter 'Bearer' followed by your JWT token in the text input below. Example: 'Bearer abc123xyz'"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] { }
                        }
                    });
                });

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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
                throw;
            }
        }
    }
}
