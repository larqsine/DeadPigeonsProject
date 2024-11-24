using DataAccess.Models;

namespace Service.DTOs.UserDto;

public class CreateUserDto
{
    public string UserName { get; set; } = String.Empty;
        
    public string FullName { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
        
    public string Phone { get; set; }= String.Empty;
    public string Password { get; set; } = String.Empty;
    public string Role { get; set; } = String.Empty;
    
    // Mapping method
    public User ToUser()
    {
        return new User
        {
            UserName = UserName,
            FullName = FullName,
            Email = Email,
            PhoneNumber = Phone,
            CreatedAt = DateTime.Now 
            
        };
    }
}