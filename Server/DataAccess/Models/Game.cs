using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Game
{
    public Guid Id { get; set; }

    public Guid AdminId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsClosed { get; set; }

    public List<int>? WinningNumbers { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal PrizePool { get; set; }

    public decimal? RolloverAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
