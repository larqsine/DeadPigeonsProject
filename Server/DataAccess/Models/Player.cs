using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models;

public partial class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public decimal? Balance { get; set; }

    public bool? AnnualFeePaid { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
