using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;


namespace Service.Interfaces;

public interface IPlayerService
{

    PlayerResponseDto CreatePlayer(PlayerCreateDto createDto);
    PlayerToClient GetPlayerById(int id);
    PlayerTransactionResponseDto AddBalance(int playerId, TransactionCreateDto transactioncreateDto, decimal amount, string mobilePayNumber);
    
    // get boards by player id
    
}
    