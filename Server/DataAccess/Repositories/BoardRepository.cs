using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class BoardRepository
    {
        private readonly DBContext _context;

        public BoardRepository(DBContext context)
        {
            _context = context;
        }

        // Retrieve a board by its ID
        public async Task<Board> GetBoardByIdAsync(Guid boardId)
        {
            return await _context.Boards
                .Include(b => b.Player)
                .Include(b => b.Game)
                .FirstOrDefaultAsync(b => b.Id == boardId);
        }

        // Retrieve all boards purchased by a player
        public async Task<List<Board>> GetBoardsByPlayerAsync(Guid playerId)
        {
            return await _context.Boards
                .Include(b => b.Game)
                .Where(b => b.PlayerId == playerId)
                .ToListAsync();
        }

        // Retrieve all boards by game
        public async Task<List<Board>> GetBoardsByGameAsync(Guid gameId)
        {
            return await _context.Boards
                .Include(b => b.Player)
                .Where(b => b.GameId == gameId)
                .ToListAsync();
        }

        // Add a new board to the database
        public async Task AddBoardAsync(Board board)
        {
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();
        }

        // Update an existing board
        public async Task UpdateBoardAsync(Board board)
        {
            _context.Boards.Update(board);
            await _context.SaveChangesAsync();
        }

        // Delete a board from the database
        public async Task DeleteBoardAsync(Guid boardId)
        {
            var board = await GetBoardByIdAsync(boardId);
            if (board != null)
            {
                _context.Boards.Remove(board);
                await _context.SaveChangesAsync();
            }
        }
    }
}
