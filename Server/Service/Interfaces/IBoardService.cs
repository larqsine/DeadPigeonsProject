using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IBoardService
    {
        // Method to allow a player to buy a board
        Task<Board> BuyBoardAsync(Guid playerId, int fieldsCount, List<int> numbers, Guid gameId);
    }
}