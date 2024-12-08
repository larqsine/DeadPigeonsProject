using DataAccess.Repositories;
using Service.DTOs.WinnerDto;
using Service.Interfaces;

namespace Service.Services;

public class WinnerService : IWinnerService
{
    private readonly WinnerRepository _winnerRepository;

    public WinnerService(WinnerRepository winnerRepository)
    {
        _winnerRepository = winnerRepository;
    }

    public async Task<List<WinnerDto>> GetWinnersByGameIdAsync(Guid gameId)
    {
        var winners = await _winnerRepository.GetWinnersByGameIdAsync(gameId);
        return winners.Select(WinnerDto.FromEntity).ToList();
    }
}