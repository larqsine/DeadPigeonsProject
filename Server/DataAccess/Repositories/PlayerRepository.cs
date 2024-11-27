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
        Console.WriteLine($"Looking for playerId: {playerId}");

        // Querying the AspNetUsers table for entities of type Player
        var player = await _context.Users
            .OfType<Player>() // EF Core automatically filters for the Player type using Discriminator internally
            .Where(u => u.Id == playerId) // Just filter by playerId
            .FirstOrDefaultAsync();

        Console.WriteLine(player == null ? "Player not found." : $"Found player: {player.Id}");
        return player;
    }

    public async Task AddPlayerAsync(Player player)
    {
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        _context.Players.Update(player);
        await _context.SaveChangesAsync();
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