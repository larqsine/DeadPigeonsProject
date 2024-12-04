using DataAccess.Models;

namespace Service.DTOs.BoardDto
{
    public class BoardResponseDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public string Numbers { get; set; }
        public bool? Autoplay { get; set; }
        
        public int? RemainingWeeks { get; set; }
        public int FieldsCount { get; set; }
        public decimal Cost { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsWinning { get; set; }

        public static BoardResponseDto FromEntity(Board board)
        {
            return new BoardResponseDto
            {
                Id = board.Id,
                PlayerId = board.PlayerId,
                GameId = board.GameId,
                Numbers = board.Numbers,
                Autoplay = board.Autoplay, 
                RemainingWeeks = board.RemainingWeeks,
                FieldsCount = board.FieldsCount,
                Cost = board.Cost,
                CreatedAt = board.CreatedAt,
                IsWinning = board.IsWinning
            };
        }
    }
}