using DataAccess.Models;
using Service.DTOs.BoardDto;

namespace Service.DTOs
{
    public static class BoardMapper
    {
        public static BoardResponseDto ToResponseDto(this Board board)
        {
            return new BoardResponseDto
            {
                Id = board.Id,
                PlayerId = board.PlayerId,
                GameId = board.GameId,
                Numbers = board.Numbers,
                FieldsCount = board.FieldsCount,
                Cost = board.Cost,
                CreatedAt = board.CreatedAt,
                IsWinning = board.IsWinning
            };
        }
    }
}