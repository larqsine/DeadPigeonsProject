using DataAccess.Models;

namespace Service.DTOs.PlayerDto
{
    public class PlayerResponseDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public decimal Balance { get; set; }
        public bool? AnnualFeePaid { get; set; }
        public DateTime? CreatedAt { get; set; }
        
        public static PlayerResponseDto FromEntity(Player player)
        {
            return new PlayerResponseDto()
            {
                Id = player.Id,
                UserName= player.UserName,
                FullName = player.FullName,
                Email = player.Email,
                Phone = player.PhoneNumber,
                Balance = player.Balance ?? 0, 
                AnnualFeePaid = player.AnnualFeePaid,
                CreatedAt = player.CreatedAt
            };
        }
    }
}
