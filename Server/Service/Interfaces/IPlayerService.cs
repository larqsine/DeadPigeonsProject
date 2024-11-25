using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;


namespace Service.Interfaces;

public interface IPlayerService
{

    Task<PlayerResponseDto> CreatePlayerAsync(PlayerCreateDto createDto);
    Task<PlayerTransactionResponseDto> AddBalanceAsync(Guid playerId, TransactionCreateDto transactionCreateDto);
    
    
}
    