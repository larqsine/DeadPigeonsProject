using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public decimal Amount { get; set; }

    public string TransactionType { get; set; } = null!;

    public string? MobilepayNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Player Player { get; set; } = null!;
}
