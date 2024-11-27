using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Models
{
    public partial class Board
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public string Numbers { get; set; } = null!;  // Store numbers as a comma-separated string
        public bool? Autoplay { get; set; }  // For repeating the board
        public int FieldsCount { get; set; }  // Number of fields chosen (5 to 8)
        public decimal Cost { get; set; }  // Calculated based on FieldsCount
        public DateTime CreatedAt { get; set; }  // Timestamp for board creation
        public bool IsWinning { get; set; }  // Indicates if this board won
        public virtual Game Game { get; set; } = null!;
        public virtual Player Player { get; set; } = null!;
        public virtual ICollection<Winner> Winners { get; set; } = new List<Winner>();

        // Constructor to calculate cost based on FieldsCount
        public Board(int fieldsCount)
        {
            if (fieldsCount < 5 || fieldsCount > 8)
                throw new ArgumentException("FieldsCount must be between 5 and 8");

            FieldsCount = fieldsCount;
            SetCost();
            CreatedAt = DateTime.UtcNow;
        }


        // Set cost based on number of fields
        public void SetCost()
        {
            switch (FieldsCount)
            {
                case 5:
                    Cost = 20m;
                    break;
                case 6:
                    Cost = 40m;
                    break;
                case 7:
                    Cost = 80m;
                    break;
                case 8:
                    Cost = 160m;
                    break;
                default:
                    throw new ArgumentException("FieldsCount must be between 5 and 8");
            }
        }

        // Determines if the board matches the winning sequence
        public bool IsWinningBoard(IEnumerable<int> winningNumbers)
        {
            var boardNumbers = Numbers.Split(',').Select(int.Parse).ToList();
            return !boardNumbers.Except(winningNumbers).Any();  // Compare numbers
        }
    }
}
