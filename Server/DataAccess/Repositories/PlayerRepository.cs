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
    
    public async Task<Player> GetByIdAsync(Guid playerId)
    {
        return await _context.Players.FindAsync(playerId); // Fetch player from DbContext
    }
    
    public async Task<Guid> GetPlayerIdByUsernameAsync(string username)
    {
        // Find the player by username using case-insensitive comparison
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.UserName.ToLower() == username.ToLower()); // Case-insensitive comparison

        if (player == null)
        {
            throw new KeyNotFoundException($"Player with username {username} not found.");
        }

        return player.Id; // Return the player's Id
    }

}