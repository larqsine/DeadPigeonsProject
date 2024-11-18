using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Board
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int GameId { get; set; }

    public string Numbers { get; set; } = null!;

    public bool? Autoplay { get; set; }

    public int FieldsCount { get; set; }

    public decimal Cost { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();
}
