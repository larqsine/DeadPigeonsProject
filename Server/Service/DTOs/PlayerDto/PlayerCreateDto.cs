using DataAccess.Models;

namespace Service.DTOs.PlayerDto
{
    public class PlayerCreateDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public bool? AnnualFeePaid { get; set; }
        
        public Player ToPlayer()
        {
            return new Player()
            {
                Name = Name,
                Email = Email,
                Phone = Phone,
                AnnualFeePaid = AnnualFeePaid,
                Balance = 0, 
                CreatedAt = DateTime.Now 
            };
        }
    }
}
