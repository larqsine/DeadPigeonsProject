    using DataAccess;
    using DataAccess.Models;
    using Microsoft.EntityFrameworkCore;
    using Service.DTOs.TransactionDto;
    using Service.DTOs.UserDto;
    using Service.Interfaces;

    namespace Service.Services;

    public class PlayerService : IPlayerService
    {
        private readonly DBContext _context;

        public PlayerService(DBContext context)
        {
            _context = context;
        }


        public PlayerResponseDto CreatePlayer(PlayerCreateDto createDto)
        {
            Player player = createDto.ToPlayer();

            try
            {
                _context.Players.Add(player);
                _context.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new DbUpdateException(e.Message);
            }
            
            return PlayerResponseDto.FromEntity(player);
        }

        public PlayerToClient GetPlayerById(Guid id)
        {
            throw new NotImplementedException();
        }

        public PlayerTransactionResponseDto AddBalance(Guid playerId, TransactionCreateDto transactioncreateDto, decimal amount, string mobilePayNumber)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("The amount to add must be greater than zero.");
            }

            var player = _context.Players.FirstOrDefault(p => p.Id == playerId);

            if (player == null)
            {
                throw new KeyNotFoundException("Player not found.");
            }
            
            var transaction = transactioncreateDto.ToTransaction();
            
            try
            {
                _context.Transactions.Add(transaction);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding balance: " + ex.Message);
            }
            player.Balance += amount;

            return new PlayerTransactionResponseDto
            {
                Player = PlayerResponseDto.FromEntity(player),
                Transaction = TransactionResponseDto.FromEntity(transaction)
            };
        }
        
        // method to get boards by player id
    }






        
