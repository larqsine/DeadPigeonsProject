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
    
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlayerAnnualFeeStatusAsync(Guid playerId, bool isPaid)
    {
        var player = await _context.Users
            .OfType<Player>()
            .FirstOrDefaultAsync(u => u.Id == playerId);

        if (player != null)
        {
            player.AnnualFeePaid = isPaid;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Player> GetByIdAsync(Guid playerId)
    {
        return await _context.Players.FindAsync(playerId); 
    }
    
    public async Task<Guid> GetPlayerIdByUsernameAsync(string username)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.UserName.ToLower() == username.ToLower());

        if (player == null)
        {
            throw new KeyNotFoundException($"Player with username {username} not found.");
        }

        return player.Id; 
    }
    public async Task<Player> GetPlayerByUsernameAsync(string username)
    {
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.UserName.ToLower() == username.ToLower()); 

        if (player == null)
        {
            throw new KeyNotFoundException($"Player with username {username} not found.");
        }

        return player; 
    }

}