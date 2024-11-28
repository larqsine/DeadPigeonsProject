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

}