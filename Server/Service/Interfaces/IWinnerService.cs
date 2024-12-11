using Service.DTOs.WinnerDto;

namespace Service.Interfaces;

public interface IWinnerService
{
    Task<List<WinnerDto>> GetWinnersByGameIdAsync(Guid gameId);
}