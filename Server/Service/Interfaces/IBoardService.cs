using Service.DTOs.BoardDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataAccess.Models;

namespace Service.Interfaces
{
    public interface IBoardService
    {
        Task<Board> CreateBoardAsync(BoardCreateDto boardDto); // Create a single board
        Task<IEnumerable<Board>> GetBoardsByPlayerAsync(Guid playerId); // Get boards by player
        Task<IEnumerable<Board>> GetBoardsByGameAsync(Guid gameId); // Get boards by game
    }
}