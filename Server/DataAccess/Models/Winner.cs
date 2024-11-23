using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Winner
{
    public Guid Id { get; set; }

    public Guid GameId { get; set; }

    public Guid PlayerId { get; set; }

    public Guid BoardId { get; set; }

    public decimal WinningAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
