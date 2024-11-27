using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class BoardRepository
    {
        private readonly DBContext _context;

        public BoardRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<Game> GetGameByIdAsync(Guid gameId)
        {
            return await _context.Games.FindAsync(gameId);
        }

        public async Task<Board> CreateBoardAsync(Board board)
        {
            _context.Boards.Add(board);
            await _context.SaveChangesAsync();
            return board;
        }

        public async Task<List<Board>> GetBoardsByPlayerIdAsync(Guid playerId)
        {
            return await _context.Boards
                .Where(b => b.PlayerId == playerId)
                .ToListAsync();
        }
    }
}