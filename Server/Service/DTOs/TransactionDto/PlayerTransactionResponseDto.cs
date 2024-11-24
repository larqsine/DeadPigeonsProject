using Service.DTOs.PlayerDto;
using Service.DTOs.UserDto;

namespace Service.DTOs.TransactionDto;

public class PlayerTransactionResponseDto
{
    public PlayerResponseDto Player { get; set; } = null!;
    public TransactionResponseDto Transaction { get; set; } = null!;
}