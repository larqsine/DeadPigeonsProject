using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class WinnerRepository
{
    private readonly DBContext _context;

    public WinnerRepository(DBContext context)
    {
        _context = context;
    }
    
    
    public async Task AddWinnersAsync(IEnumerable<Winner> winners)
    {
        _context.Winners.AddRange(winners);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Winner?> GetWinnerByIdAsync(Guid id)
    {
        return await _context.Winners
            .Include(w => w.Game)
            .Include(w => w.Player)
            .Include(w => w.Board)
            .FirstOrDefaultAsync(w => w.Id == id);
    }
    
    public async Task<List<Winner>> GetWinnersByGameIdAsync(Guid gameId)
    {
        return await _context.Winners
            .Include(w => w.Game)
            .Include(w => w.Player)
            .Include(w => w.Board)
            .Where(w => w.GameId == gameId)
            .ToListAsync();
    }
    
    public async Task UpdateWinnerAsync(Winner winner)
    {
        _context.Winners.Update(winner);
        await _context.SaveChangesAsync();
    }
    
   
    
  
}
