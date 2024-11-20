using Service.DTOs.playerDto;
using Service.DTOs.PlayerDTO;

namespace Service.Repositories;

public interface IPlayerRepo
{

    PlayerResponseDto CreatePlayer(PlayerCreateDto createDto);
    PlayerToClient GetPlayerById(int id);
    PlayerResponseDto AddBalance(int playerId, decimal amount, string mobilePayNumber);

    //List<BoardDto> GetBoardsByPlayerId(int playerId);
}
    