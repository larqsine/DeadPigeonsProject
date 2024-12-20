using DataAccess.Models;

namespace Service.DTOs.PlayerDto;

public class PlayerUpdateDto
{
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool? AnnualFeePaid { get; set; }
    public decimal? Balance { get; set; }

    // Mapping method to update the Player entity
    public void UpdatePlayer(Player player)
    {
        if (!string.IsNullOrEmpty(UserName))
            player.UserName = UserName;
        if (!string.IsNullOrEmpty(FullName))
            player.FullName = FullName;
        if (!string.IsNullOrEmpty(Email))
            player.Email = Email;
        if (!string.IsNullOrEmpty(Phone))
            player.PhoneNumber = Phone;
        if (AnnualFeePaid.HasValue)
            player.AnnualFeePaid = AnnualFeePaid.Value;
        
    }
}