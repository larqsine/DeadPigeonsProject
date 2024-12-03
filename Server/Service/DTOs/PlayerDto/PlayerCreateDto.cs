using DataAccess.Models;
using Service.DTOs.UserDto;


namespace Service.DTOs.PlayerDto;

    public class PlayerCreateDto : CreateUserDto
    {
        public bool? AnnualFeePaid { get; set; }

        // Mapping method
        public Player ToPlayer()
        {
            return new Player
            {
                UserName = UserName,
                FullName = FullName,
                Email = Email,
                PhoneNumber = Phone,
                AnnualFeePaid = true,
                Balance = 0,
                CreatedAt = DateTime.Now
            };

        }
    }

