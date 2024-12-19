using DataAccess.Enums;
using DataAccess.Repositories;
using Service.DTOs.TransactionDto;
using Service.Interfaces;

namespace Service.Services
{
    public class TransactionService: ITransactionService
    {
        private readonly TransactionRepository _transactionRepository;
        private readonly PlayerRepository _playerRepository;

        public TransactionService(TransactionRepository transactionRepository, PlayerRepository playerRepository)
        {
            _transactionRepository = transactionRepository;
            _playerRepository = playerRepository;
        }
        
        public async Task<TransactionResponseDto?> ApproveTransactionAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return null; 
            }
            
            transaction.Status = TransactionStatus.Approved;
            
            var player = await _playerRepository.GetPlayerByIdAsync(transaction.PlayerId);  
            if (player != null) 
            {
              
                player.Balance += transaction.Amount;
                await _playerRepository.SaveAsync(); 
            }
            
            await _transactionRepository.SaveAsync();  

            return TransactionResponseDto.FromEntity(transaction);
        }

        public async Task<TransactionResponseDto?> DeclineTransactionAsync(Guid transactionId)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return null; 
            }
            
            transaction.Status = TransactionStatus.Declined;
            
            await _transactionRepository.SaveAsync();

            return TransactionResponseDto.FromEntity(transaction);
            
        }
        public async Task<List<TransactionResponseDto>> GetTransactionsByTypeAsync(string type)
        {
            var transactions = await _transactionRepository.GetTransactionsByTypeAsync(type);

            return transactions.Select(TransactionResponseDto.FromEntity).ToList();
        }


        
        public async Task<List<TransactionResponseDto>> GetTransactionsByPlayerIdAsync(Guid playerId)
        {
            var transactions = await _transactionRepository.GetTransactionsByPlayerIdAsync(playerId);
            return transactions.Select(TransactionResponseDto.FromEntity).ToList();
        }
    }
}