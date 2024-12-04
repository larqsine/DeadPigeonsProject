using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IPlayerService
    {

        Task<PlayerResponseDto> GetPlayerByIdAsync(Guid playerId);

        Task<List<PlayerResponseDto>> GetAllPlayersAsync();

        Task<PlayerResponseDto> UpdatePlayerAsync(Guid playerId, PlayerUpdateDto updateDto);

        Task DeletePlayerAsync(Guid playerId);

        Task<Guid> GetPlayerIdByUsernameAsync(string username); 
        Task<PlayerTransactionResponseDto> AddBalanceAsync(Guid playerId, TransactionCreateDto transactionCreateDto);
        
        Task<decimal> GetPlayerBalanceAsync(Guid playerId);
    }
}