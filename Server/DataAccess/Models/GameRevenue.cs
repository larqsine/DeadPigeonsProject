using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class GameRevenue
{
    public int GameId { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal PrizePool { get; set; }

    public decimal? RolloverAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;
}
