using DataAccess.Models;
using Service.DTOs.WinnerDto;

namespace Service.Interfaces;

public interface IGameService
{
    Task<Game> StartNewGameAsync(Guid adminId);

    Task<List<WinnerDto>> CloseGameAsync(Guid gameId, List<int> winningNumbers);

    bool IsWinningBoard(List<int> boardNumbers, List<int> winningNumbers);
}