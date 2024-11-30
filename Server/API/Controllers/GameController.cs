
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame()
        {
            try
            {
                var game = await _gameService.StartNewGameAsync();
                return Ok(new
                {
                    message = "Game started successfully.",
                    data = game
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("{gameId}/close")]
        public async Task<IActionResult> CloseGame(Guid gameId, [FromBody] List<int> winningNumbers)
        {
            try
            {
                await _gameService.CloseGameAsync(gameId, winningNumbers);
                return Ok(new { message = "Game closed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}