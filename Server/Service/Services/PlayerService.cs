using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;
using Service.Interfaces;

namespace Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly PlayerRepository _repository;
        private readonly UserManager<User> _userManager;

        
        public PlayerService(PlayerRepository repository, UserManager<User> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        
        
        public async Task<PlayerResponseDto> GetPlayerByIdAsync(Guid playerId)
        {
            try
            {
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }
                return PlayerResponseDto.FromEntity(player);
            }
            catch (KeyNotFoundException ex)
            {
                LogError($"Player not found for ID: {playerId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                LogError("GetPlayerByIdAsync failed", ex);
                throw new ApplicationException("An error occurred while retrieving the player.");
            }
        }
        public async Task<List<PlayerResponseDto>> GetAllPlayersAsync()
        {
            try
            {
                var players = await _repository.GetAllPlayersAsync();
                return players.Select(PlayerResponseDto.FromEntity).ToList();
            }
            catch (Exception ex)
            {
                LogError("GetAllPlayersAsync failed", ex);
                throw new ApplicationException("An error occurred while retrieving players.");
            }
        }
        public async Task<PlayerResponseDto> UpdatePlayerAsync(Guid playerId, PlayerUpdateDto updateDto)
        {
            try
            {
                // Retrieve the player by ID
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }

                // If email is being updated, validate it
                if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != player.Email)
                {
                    // Validate email with UserManager
                    var user = await _userManager.FindByEmailAsync(updateDto.Email);
                    if (user != null)
                    {
                        throw new ApplicationException("Email is already in use.");
                    }

                    // If the email is valid, update it
                    var result = await _userManager.SetEmailAsync(player, updateDto.Email);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new ApplicationException($"Email update failed: {errors}");
                    }
                }

                // Update other fields using the DTO
                updateDto.UpdatePlayer(player);

                // Save changes to the player
                await _repository.UpdatePlayerAsync(player);
                return PlayerResponseDto.FromEntity(player);
            }
            catch (KeyNotFoundException ex)
            {
                LogError($"Player not found for ID: {playerId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                LogError("UpdatePlayerAsync failed", ex);
                throw new ApplicationException("An error occurred while updating the player.");
            }
        }
        public async Task DeletePlayerAsync(Guid playerId)
        {
            try
            {
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }

                await _repository.DeletePlayerAsync(playerId);
            }
            catch (KeyNotFoundException ex)
            {
                LogError($"Player not found for ID: {playerId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                LogError("DeletePlayerAsync failed", ex);
                throw new ApplicationException("An error occurred while deleting the player.");
            }
        }
        public async Task<PlayerTransactionResponseDto> AddBalanceAsync(
            Guid playerId,
            TransactionCreateDto transactionCreateDto)
        {
            try
            {
                if (transactionCreateDto.Amount <= 0)
                {
                    throw new ArgumentException("The amount to add must be greater than zero.");
                }

                // Ensure the player exists
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }

                // Generate Transaction ID
                var transactionId = Guid.NewGuid();

                // Create and save the transaction with default status as Pending
                var transaction = transactionCreateDto.ToDepositTransaction(playerId, transactionId);
                transaction.Status = TransactionStatus.Pending; // Default status
                await _repository.AddTransactionAsync(transaction);

                // Return the transaction details without modifying the balance
                return new PlayerTransactionResponseDto
                {
                    Player = PlayerResponseDto.FromEntity(player),
                    Transaction = TransactionResponseDto.FromEntity(transaction)
                };
            }
            catch (ArgumentException ex)
            {
                LogError("Invalid transaction amount", ex);
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                LogError($"Player not found for ID: {playerId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                LogError("AddBalanceAsync failed", ex);
                throw new ApplicationException("An error occurred while adding balance to the player.");
            }
        }
        public async Task<decimal> GetPlayerBalanceAsync(Guid playerId)
        {
            try
            {
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }

                return player.Balance ?? 0; 
            }
            catch (KeyNotFoundException ex)
            {
                LogError($"Player not found for ID: {playerId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                LogError("GetPlayerBalanceAsync failed", ex);
                throw new ApplicationException("An error occurred while retrieving the player's balance.");
            }
        }




        private void LogError(string message, Exception ex)
        {
            // Logs inner exceptions (detailed)
            Console.WriteLine($"{message}: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
        }
    }
}
