namespace Service.DTOs.BoardDto
{
    public class BoardResponseDto
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerFullName { get; set; } = string.Empty; // Player's full name for convenience
        public Guid GameId { get; set; }
        public string GameStartDate { get; set; } = string.Empty; // Game's start date in string format
        public string Numbers { get; set; } = string.Empty; // The numbers on the board as a comma-separated string
        public bool? Autoplay { get; set; }
        public int FieldsCount { get; set; }
        public decimal Cost { get; set; } // The cost of the board based on the fields selected
        public DateTime? CreatedAt { get; set; } // Timestamp when the board was created
        public bool IsWinning { get; set; } // Whether the board is a winning board
    }
}