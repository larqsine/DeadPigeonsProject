using Service.DTOs.TransactionDto;

namespace Service.Interfaces;

public interface ITransactionService
{
    Task<TransactionResponseDto?> ApproveTransactionAsync(Guid transactionId);
    Task<TransactionResponseDto?> DeclineTransactionAsync(Guid transactionId);
}