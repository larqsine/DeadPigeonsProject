using DataAccess.Enums;
using DataAccess.Models;

namespace Service.DTOs.TransactionDto
{
    public class TransactionResponseDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = null!;
        public string? MobilepayNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public TransactionStatus Status { get; set; }
        
        public static TransactionResponseDto FromEntity(Transaction transaction)
        {
            return new TransactionResponseDto()
            {
                Id = transaction.Id,
                PlayerId = transaction.PlayerId,
                Amount = transaction.Amount,
                TransactionType = transaction.TransactionType,
                MobilepayNumber = transaction.MobilepayNumber,
                CreatedAt = transaction.CreatedAt,
                Status = transaction.Status
            };
        }
    }
}