namespace Service.DTOs.GameDto;

public class GameDetailsDto
{
    public Guid Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal PrizePool { get; set; }
    public bool IsClosed { get; set; }
    public List<int>? WinningNumbers { get; set; }
}