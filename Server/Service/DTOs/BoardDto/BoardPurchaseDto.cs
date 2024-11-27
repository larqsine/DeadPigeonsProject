namespace Service.DTOs.BoardDto
{
    public class BoardPurchaseDto
    {
        public Guid PlayerId { get; set; }  // Player purchasing the board(s)
        public List<BoardCreateDto> Boards { get; set; } = new List<BoardCreateDto>();  // List of boards to purchase
    }
}