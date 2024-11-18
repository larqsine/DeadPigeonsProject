using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Game
{
    public int Id { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsClosed { get; set; }

    public string? WinningNumbers { get; set; }

    public decimal? PrizePool { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual GameRevenue? GameRevenue { get; set; }

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
