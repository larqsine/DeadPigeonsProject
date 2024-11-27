using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public class Admin : User
{
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
