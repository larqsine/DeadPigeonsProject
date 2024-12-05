using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DTOs.BoardDto;

namespace Service.Interfaces
{
    public interface IBoardService
    {
        Task<BoardResponseDto> BuyBoardAsync(Guid playerId, BuyBoardRequestDto buyBoardRequestDto);
        Task<List<BoardResponseDto>> GetBoardsByPlayerIdAsync(Guid playerId);
        Task<List<BoardResponseDto>> GetAllBoardsAsync();
        Task<List<BoardResponseDto>> GetBoardsByGameIdAsync(Guid gameId);
    }
}