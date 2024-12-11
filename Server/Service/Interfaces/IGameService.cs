using DataAccess.Models;
using Service.DTOs.GameDto;
using Service.DTOs.WinnerDto;

namespace Service.Interfaces;

public interface IGameService
{
    
        Task<Game?> GetActiveGameAsync();
        Task<List<GameResponseDto>> GetAllGamesAsync();
        Task<GameDetailsDto> StartNewGameAsync();
        Task<List<WinnerDto>> CloseGameAsync(Guid gameId, List<int> winningNumbers);
        bool IsWinningBoard(List<int> boardNumbers, List<int> winningNumbers);
}