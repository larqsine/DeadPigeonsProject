using DataAccess.Models;

namespace Service.DTOs.WinnerDto;

public class WinnerDto
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
    public Guid BoardId { get; set; }
    public decimal WinningAmount { get; set; }
    public DateTime? CreatedAt { get; set; }

    public static WinnerDto FromEntity(Winner winner)
    {
        return new WinnerDto
        {
            Id = winner.Id,
            GameId = winner.GameId,
            PlayerId = winner.PlayerId,
            BoardId = winner.BoardId,
            WinningAmount = winner.WinningAmount,
            CreatedAt = winner.CreatedAt
        };
    }

    public Winner ToEntity()
    {
        return new Winner
        {
            Id = Id,
            GameId = GameId,
            PlayerId = PlayerId,
            BoardId = BoardId,
            WinningAmount = WinningAmount,
            CreatedAt = CreatedAt
        };
    }
}