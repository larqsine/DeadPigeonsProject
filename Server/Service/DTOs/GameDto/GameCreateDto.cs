namespace Service.DTOs.GameDto
{
    public class GameCreateDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}