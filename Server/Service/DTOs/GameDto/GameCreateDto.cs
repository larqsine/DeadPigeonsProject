namespace Service.DTOs.GameDto;

public class GameCreateDto
{
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

}