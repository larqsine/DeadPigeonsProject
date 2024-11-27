using DataAccess.Models;
using TransactionStatus = DataAccess.Enums.TransactionStatus;
namespace Service.DTOs.TransactionDto;

public class TransactionUpdateDto
{
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    public Transaction ToUpdatedTransaction(Transaction transaction)
    {
        transaction.Status = Status;
        return transaction;
    }
}