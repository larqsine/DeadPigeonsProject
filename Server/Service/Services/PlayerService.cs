using DataAccess.Models;
using DataAccess.Repositories;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;
using Service.Interfaces;

namespace Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly PlayerRepository _repository;

        public PlayerService(PlayerRepository repository)
        {
            _repository = repository;
        }

        public async Task<PlayerResponseDto> CreatePlayerAsync(PlayerCreateDto createDto)
        {
            var player = createDto.ToPlayer();

            await _repository.AddPlayerAsync(player);
            return PlayerResponseDto.FromEntity(player);
        }
        
        public async Task<PlayerTransactionResponseDto> AddBalanceAsync(
            Guid playerId,
            TransactionCreateDto transactionCreateDto)
        {
            if (transactionCreateDto.Amount <= 0)
            {
                throw new ArgumentException("The amount to add must be greater than zero.");
            }

            try
            {
                // Ensure that the player exists and is of type "Player"
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }

                // Create and save the transaction
                var transaction = transactionCreateDto.ToTransaction(playerId);
                await _repository.AddTransactionAsync(transaction);

                // Update player balance
                player.Balance += transactionCreateDto.Amount;
                await _repository.UpdatePlayerAsync(player);

                return new PlayerTransactionResponseDto
                {
                    Player = PlayerResponseDto.FromEntity(player),
                    Transaction = TransactionResponseDto.FromEntity(transaction)
                };
            }
            catch (Exception ex)
            {
                // Log the error details
                Console.WriteLine($"Error in AddBalanceAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }


    }
}