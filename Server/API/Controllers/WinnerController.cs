using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WinnerController : ControllerBase
{
    private readonly IWinnerService _winnerService;


    public WinnerController(IWinnerService winnerService)
    {
        _winnerService = winnerService;
    }

    [HttpGet("games/{gameId:guid}/GetAllWinners")]
    public async Task<IActionResult> GetWinnersByGameId(Guid gameId)
    {
        try
        {
            var winners = await _winnerService.GetWinnersByGameIdAsync(gameId);

            if (winners == null || !winners.Any())
                return NotFound(new { message = "No winners found for the specified game." });

            return Ok(winners);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}