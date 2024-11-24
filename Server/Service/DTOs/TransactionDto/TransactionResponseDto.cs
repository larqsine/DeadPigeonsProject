using DataAccess.Models;

namespace Service.DTOs.TransactionDto;

public class TransactionResponseDto
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = null!;
    public string MobilepayNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    
    
    public static TransactionResponseDto FromEntity(Transaction transaction)
    {
        return new TransactionResponseDto()
        {
            Id = transaction.Id,
            PlayerId = transaction.PlayerId,
            Amount = transaction.Amount,
            TransactionType = transaction.TransactionType,
            MobilepayNumber = transaction.MobilepayNumber,
            CreatedAt = transaction.CreatedAt
        };
    }
}