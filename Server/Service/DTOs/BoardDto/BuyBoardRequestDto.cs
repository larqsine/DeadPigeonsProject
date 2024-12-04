using System;
using System.Collections.Generic;
using DataAccess.Models;

namespace Service.DTOs.BoardDto
{
    public class BuyBoardRequestDto
    {
        public int FieldsCount { get; set; }  // Number of fields selected (5-8)
        public List<int> Numbers { get; set; }  // The numbers selected for the board
        public Guid GameId { get; set; }
        public int Weeks { get; set; } = 0;
        // The ToBoard method that maps the DTO to the Board entity
        public Board ToBoard(Guid playerId, decimal cost)
        {
            return new Board
            {
                Id = Guid.NewGuid(),
                PlayerId = playerId,
                GameId = GameId,
                Numbers = string.Join(",", Numbers),
                FieldsCount = FieldsCount,
                Cost = cost,
                RemainingWeeks = Weeks,
                Autoplay = Weeks > 0, 
                CreatedAt = DateTime.UtcNow,
                IsWinning = false
            };
        }
    }
}