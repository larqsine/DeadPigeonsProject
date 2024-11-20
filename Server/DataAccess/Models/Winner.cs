using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Winner
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public int PlayerId { get; set; }

    public int BoardId { get; set; }

    public decimal WinningAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
