using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.Interfaces;

namespace Service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly PlayerRepository _repository;
        private readonly TransactionRepository _transactionRepository;
        private readonly UserManager<User> _userManager;


        public PlayerService(PlayerRepository repository,
            TransactionRepository transactionRepository,
            UserManager<User> userManager)
        {
            _repository = repository;
            _userManager = userManager;
            _transactionRepository = transactionRepository;
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
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }
                
                if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != player.Email)
                {
                    var user = await _userManager.FindByEmailAsync(updateDto.Email);
                    if (user != null)
                    {
                        throw new ApplicationException("Email is already in use.");
                    }
                    
                    var result = await _userManager.SetEmailAsync(player, updateDto.Email);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new ApplicationException($"Email update failed: {errors}");
                    }
                }
                
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
                
                var player = await _repository.GetPlayerByIdAsync(playerId);
                if (player == null)
                {
                    throw new KeyNotFoundException("Player not found.");
                }
                
                var transactionId = Guid.NewGuid();

                // Create and save the transaction with default status as Pending
                var transaction = transactionCreateDto.ToDepositTransaction(playerId, transactionId);
                transaction.Status = TransactionStatus.Pending; // Default status
                await _transactionRepository.AddTransactionAsync(transaction);

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
        public async Task<PlayerResponseDto> TogglePlayerActiveStatusAsync(Guid playerId, bool isActive)
        {
            try
            {
                await _repository.UpdatePlayerAnnualFeeStatusAsync(playerId, isActive);

                // Retrieve the updated player and return the PlayerResponseDto
                var player = await _repository.GetPlayerByIdAsync(playerId);
                return PlayerResponseDto.FromEntity(player);  // Convert player to DTO
            }
            catch (KeyNotFoundException ex)
            {
                LogError($"Player not found for ID: {playerId}", ex);
                throw;
            }
            catch (Exception ex)
            {
                LogError("TogglePlayerActiveStatusAsync failed", ex);
                throw new ApplicationException("An error occurred while toggling the player status.");
            }
        }

        public async Task<Guid> GetPlayerIdByUsernameAsync(string username)
        {
            try
            {
                // Call the repository to get the player ID by username
                return await _repository.GetPlayerIdByUsernameAsync(username);
            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Player with username {username} not found.", ex);
            }
        }

        
        public async Task<PlayerResponseDto> GetPlayerByUsernameAsync(string username)
        {
            try
            {
                // Call the repository to get the player ID by username
                var player = await _repository.GetPlayerByUsernameAsync(username);
                return PlayerResponseDto.FromEntity(player);

            }
            catch (KeyNotFoundException ex)
            {
                throw new KeyNotFoundException($"Player with username {username} not found.", ex);
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
