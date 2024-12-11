

using DataAccess.Models;

namespace Service.DTOs.GameDto
{
    public class GameResponseDto
    {
        public Guid Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public bool IsClosed { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal PrizePool { get; set; }
        public decimal? RolloverAmount { get; set; }
        public List<int>? WinningNumbers { get; set; }

        public static GameResponseDto FromEntity(Game game)
        {
            return new GameResponseDto
            {
                Id = game.Id,
                StartDate = game.StartDate,
                EndDate = game.EndDate,
                IsClosed = game.IsClosed ?? false,
                TotalRevenue = game.TotalRevenue,
                PrizePool = game.PrizePool,
                RolloverAmount = game.RolloverAmount,
                WinningNumbers = game.WinningNumbers!
            };
        }
}
    
}