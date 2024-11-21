using DataAccess.Models;

namespace Service.DTOs.PlayerDto
{
    public class PlayerResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
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
                Name = player.Name,
                Email = player.Email,
                Phone = player.Phone,
                Balance = player.Balance ?? 0, 
                AnnualFeePaid = player.AnnualFeePaid,
                CreatedAt = player.CreatedAt
            };
        }
    }
}
