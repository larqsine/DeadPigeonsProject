using System.Text.Json.Serialization;
using DataAccess.Enums;
using DataAccess.Models;

namespace Service.DTOs.TransactionDto;

public class TransactionCreateDto
{
    public decimal Amount { get; set; } 
    public string MobilepayNumber { get; set; } 
    
    public Transaction ToDepositTransaction(Guid playerId, Guid transactionId)
    {
        return new Transaction
        {
            Id = transactionId,
            Amount = Amount,
            TransactionType = "deposit",
            MobilepayNumber = MobilepayNumber,
            CreatedAt = DateTime.UtcNow,
            PlayerId = playerId,
            Status = TransactionStatus.Pending // Default status is Pending

        };
    }
    
    // Creates a purchase transaction for buying a board
    public Transaction ToPurchaseTransaction(Guid playerId, Guid transactionId)
    {
        return new Transaction
        {
            Id = transactionId,
            Amount = -Amount, // Deducted amount for purchase
            TransactionType = "purchase",
            CreatedAt = DateTime.UtcNow,
            PlayerId = playerId,
            Status = TransactionStatus.Completed // Purchase transactions are marked as completed
        };
    }

}