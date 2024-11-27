namespace Service.DTOs.BoardDto
{
    public class WinnerDto
    {
        public Guid WinnerId { get; set; } // The player's ID who is a winner
        public string WinnerFullName { get; set; } = string.Empty; // Winner's full name
        public decimal PrizeAmount { get; set; } // Prize amount for this winner
    }
}