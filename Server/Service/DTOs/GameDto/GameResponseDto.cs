using Service.DTOs.BoardDto;

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
    
    }
}