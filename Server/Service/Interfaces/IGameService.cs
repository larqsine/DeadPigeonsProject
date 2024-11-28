using DataAccess.Models;


namespace Service.Interfaces
{
    public interface IGameService
    {
        Task<Game> StartNewGameAsync(Guid adminId);

        Task CloseGameAsync(Guid gameId, List<int> winningNumbers);

        bool IsWinningBoard(string boardNumbers, List<int> winningNumbers);
    }
}