using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;

namespace Service.Interfaces
{
    public interface IPlayerService
    {
        Task<PlayerResponseDto> GetPlayerByIdAsync(Guid playerId);
        Task<PlayerResponseDto> GetPlayerByUsernameAsync(string username);
        Task<List<PlayerResponseDto>> GetAllPlayersAsync();
        Task<PlayerResponseDto> UpdatePlayerAsync(Guid playerId, PlayerUpdateDto updateDto);
        Task DeletePlayerAsync(Guid playerId);
        Task<PlayerResponseDto> TogglePlayerActiveStatusAsync(Guid playerId, bool isActive);
        Task<Guid> GetPlayerIdByUsernameAsync(string username); 
        Task<PlayerTransactionResponseDto> AddBalanceAsync(Guid playerId, TransactionCreateDto transactionCreateDto);
        Task<decimal> GetPlayerBalanceAsync(Guid playerId);
    }
}