using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class PlayerRepository
{
    private readonly DBContext _context;

    public PlayerRepository(DBContext context)
    {
        _context = context;
    }
    public async Task<Player?> GetPlayerByIdAsync(Guid playerId)
    {

        var player = await _context.Users
            .OfType<Player>() // EF Core automatically filters for the Player type using Discriminator internally
            .Where(u => u.Id == playerId) 
            .FirstOrDefaultAsync();

        return player;
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        var players = await _context.Users.OfType<Player>().ToListAsync();

        return players;
    }
    

    public async Task UpdatePlayerAsync(Player player)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePlayerAsync(Guid playerId)
    {
        var player = await GetPlayerByIdAsync(playerId);
        if (player != null)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddTransactionAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

}