using Microsoft.AspNetCore.Identity;

namespace DataAccess.Models;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
}