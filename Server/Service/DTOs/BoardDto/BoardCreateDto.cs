namespace Service.DTOs.BoardDto
{
    public class BoardCreateDto
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public string Numbers { get; set; }  // Comma-separated string of numbers (e.g., "1,2,3,4,5")
        public int FieldsCount { get; set; }  // Number of fields selected by the player (5-8)
        public bool? Autoplay { get; set; } 
    }
}