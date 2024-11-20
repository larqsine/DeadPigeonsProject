using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Service.DTOs.playerDto;
using Service.Repositories;
using Service.DTOs.PlayerDTO;

namespace Service.Repositories.Player;

public class PlayerRepo : IPlayerRepo
{
    private readonly DBContext _context;

    public PlayerRepo(DBContext context)
    {
        _context = context;
    }


    public PlayerResponseDto CreatePlayer(PlayerCreateDto createDto)
    {
        DataAccess.Models.Player player = createDto.ToPlayer();

        try
        {
            _context.Player.Add(player);
            _context.SaveChanges();
        }
        catch (DbUpdateException e)
        {
            throw new DbUpdateException(e.Message);
        }
        
        return PlayerResponseDto.FromEntity(player);
    }

    public PlayerToClient GetPlayerById(int id)
    {
        throw new NotImplementedException();
    }

    public PlayerResponseDto AddBalance(int playerId, decimal amount, string mobilePayNumber)
    {
       /* if (amount <= 0)
            throw new ArgumentException("The amount to add must be greater than zero.");
    
        var player = _context.Player.FirstOrDefault(p => p.Id == playerId);
        if (player == null)
            throw new KeyNotFoundException("Player not found.");
    
        var transaction = new Transaction
        {
            PlayerId = playerId,
            Amount = amount,
            TransactionType = "Deposit",
            MobilePayNumber = mobilePayNumber,
            CreatedAt = DateTime.UtcNow
        };
    
        player.Balance += amount;

        try
        {
            _context.Transaction.Add(transaction);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Error occurred while adding balance: " + ex.Message);
        }

        return PlayerResponseDto.FromEntity(player);*/
       return null;
    }

   /* public List<BoardDto> GetBoardsByPlayerId(int playerId)
    {
        throw new NotImplementedException();
    }*/
}


    
