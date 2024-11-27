namespace Service.DTOs
{
    public class BuyBoardRequestDto
    {
        public int FieldsCount { get; set; }  // Number of fields selected (5-8)
        public List<int> Numbers { get; set; }  // The numbers selected for the board
        public Guid GameId { get; set; } 

    }
}