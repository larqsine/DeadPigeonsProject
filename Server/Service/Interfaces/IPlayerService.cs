using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;


namespace Service.Interfaces;

public interface IPlayerService
{

    PlayerResponseDto CreatePlayer(PlayerCreateDto createDto);
    PlayerToClient GetPlayerById(Guid id);
    PlayerTransactionResponseDto AddBalance(Guid playerId, TransactionCreateDto transactioncreateDto, decimal amount, string mobilePayNumber);
    
    // get boards by player id
    
}
    