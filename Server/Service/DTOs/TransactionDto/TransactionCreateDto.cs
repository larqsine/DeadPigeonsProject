using DataAccess.Models;

namespace Service.DTOs.TransactionDto;

public class TransactionCreateDto
{
    //public Guid PlayerId { get; set; } 
    public decimal Amount { get; set; } 
    public string TransactionType { get; set; } = null!;
    public string MobilepayNumber { get; set; } 
    
    public Transaction ToTransaction(Guid id)
    {
        return new Transaction
        {
            //PlayerId = PlayerId,
            Amount = Amount,
            TransactionType = TransactionType,
            MobilepayNumber = MobilepayNumber,
            CreatedAt = DateTime.UtcNow
        };
    }

}