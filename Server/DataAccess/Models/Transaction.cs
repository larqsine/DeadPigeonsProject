using System;
using System.Collections.Generic;
using System.Transactions;
using TransactionStatus = DataAccess.Enums.TransactionStatus;

namespace DataAccess.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = null!;

    public string? MobilepayNumber { get; set; }

    public DateTime? CreatedAt { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public virtual Player Player { get; set; } = null!;
}
