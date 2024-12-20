using System;
using System.Collections.Generic;
using DataAccess.Models;

namespace Service.DTOs.BoardDto
{
    public class BuyBoardRequestDto
    {
        public int FieldsCount { get; set; }
        public List<int> Numbers { get; set; } 
        public Guid GameId { get; set; }
        public int RemainingAutoplayWeeks { get; set; }

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
                CreatedAt = DateTime.UtcNow,
                IsWinning = false,
                RemainingAutoplayWeeks = RemainingAutoplayWeeks
            };
        }
    }
}