﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Models
{
    public partial class Board
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public string Numbers { get; set; } = null!; 
        public bool? Autoplay { get; set; }  
        public int FieldsCount { get; set; }  
        public decimal Cost { get; set; } 
        public DateTime CreatedAt { get; set; }  
        public bool IsWinning { get; set; }

        public virtual Game Game { get; set; } = null!;
        public virtual Player Player { get; set; } = null!;
        public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();

        
        public bool IsWinningBoard(IEnumerable<int> winningNumbers)
        {
            var boardNumbers = Numbers.Split(',').Select(int.Parse).ToList();
            return !boardNumbers.Except(winningNumbers).Any();  
        }
    }
}
