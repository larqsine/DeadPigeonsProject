using Service.DTOs.PlayerDto;

namespace Service.DTOs.TransactionDto;

public class PlayerTransactionResponseDto
{
    public PlayerResponseDto Player { get; set; } = null!;
    public TransactionResponseDto Transaction { get; set; } = null!;
}