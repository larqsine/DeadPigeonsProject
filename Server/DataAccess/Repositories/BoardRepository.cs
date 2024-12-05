using System.Collections;
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

        public async Task<List<Board>> GetBoardsByGameIdAsync(Guid gameId, bool groupByPlayer = false)
        {
            var query = _context.Boards
                .Include(b => b.Player)
                .Where(b => b.GameId == gameId);

            if (groupByPlayer)
            {
                query = query.OrderBy(b => b.Player.FullName);
            }

            return await query.ToListAsync();
        }
        
        public async Task UpdateBoardsAsync(IEnumerable<Board> boards)
        {
            foreach (var board in boards)
            {
                _context.Entry(board).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<Board>> GetBoardsForAutoplayAsync()
        {
            return await _context.Boards
                .Where(b => b.Autoplay && b.RemainingAutoplayWeeks > 0)
                .ToListAsync();
        }



    }
}