namespace Service.DTOs.BoardDto;

public class PlayerBoardsSummaryDto
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int TotalBoards { get; set; }
}
