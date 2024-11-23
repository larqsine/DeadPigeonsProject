using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public  class Player : User
{
    public decimal? Balance { get; set; }

    public bool? AnnualFeePaid { get; set; }
    
    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
