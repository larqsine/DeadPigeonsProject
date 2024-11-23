using Microsoft.AspNetCore.Identity;

namespace DataAccess.Models;

public class User : IdentityUser<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? CreatedAt { get; set; }
}