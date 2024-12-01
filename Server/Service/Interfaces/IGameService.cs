using DataAccess.Models;
using Service.DTOs.GameDto;


namespace Service.Interfaces
{
    public interface IGameService
    {
        Task<Game?> GetActiveGameAsync();
        Task<GameDetailsDto> StartNewGameAsync();
        Task CloseGameAsync(Guid gameId, List<int> winningNumbers);
    }
}