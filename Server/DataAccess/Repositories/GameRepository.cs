using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class GameRepository
{
        private readonly DBContext _context;

        public GameRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<Game> CreateGameAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
            return game;
        }
        
        public async Task<Game> GetGameByIdAsync(Guid gameId)
        {
            return await _context.Games.FindAsync(gameId);
        }

        public async Task<Game?> GetActiveGameAsync()
        {
            return await _context.Games.FirstOrDefaultAsync(g => g.IsClosed == false);
        }


        public async Task CloseGameAsync(Guid gameId, List<int> winningNumbers, decimal rolloverAmount)
        {
            var game = await _context.Games.FindAsync(gameId);

            if (game == null)
                throw new Exception("Game not found");

            game.IsClosed = true;
            game.EndDate = DateOnly.FromDateTime(DateTime.UtcNow);
            game.WinningNumbers = winningNumbers;
            game.RolloverAmount = rolloverAmount;

            await _context.SaveChangesAsync();
        }
    
        public async Task UpdateGameAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
}