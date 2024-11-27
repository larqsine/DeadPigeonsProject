using DataAccess.Models;


namespace Service.DTOs.UserDto;

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
                AnnualFeePaid = AnnualFeePaid,
                Balance = 0,
                CreatedAt = DateTime.Now
            };

        }
    }

