using System.ComponentModel.DataAnnotations;

namespace Service;

public sealed class AppOptions
{
    [Required] public required string JwtSecret { get; set; } // For JWT authentication
    [Required] public required string DbConnectionString { get; set; } // Database connection string
    public required string Address { get; set; } // Application address for external APIs or references
    public bool RunInTestContainer { get; set; } = false; // Used for distinguishing between test and production containers
    public bool Seed { get; set; } = true; // Whether to seed the database during initialization
}