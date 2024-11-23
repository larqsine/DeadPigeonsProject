using DataAccess.Models;

namespace Service.DTOs.PlayerDto
{
    public class PlayerCreateDto
    {
        public string FullName { get; set; } = null!;
        
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public bool? AnnualFeePaid { get; set; }
        
        public Player ToPlayer()
        {
            return new Player()
            {
                UserName = UserName,
                FullName = FullName,
                Email = Email,
                PhoneNumber = Phone,
                AnnualFeePaid = AnnualFeePaid,
                Balance = 0, 
                CreatedAt = DateTime.Now 
            };
        }
    }
}
