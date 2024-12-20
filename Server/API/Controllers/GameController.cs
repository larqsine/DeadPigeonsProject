
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.GameDto;
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
        public async Task<IActionResult> StartGame([FromBody] GameCreateDto gameCreateDto)
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
        public async Task<IActionResult> CloseGame(Guid gameId, [FromBody] GameCloseDto gameCloseDto)
        {
            if (gameCloseDto.WinningNumbers == null || !gameCloseDto.WinningNumbers.Any())
            {
                return BadRequest(new { message = "Winning numbers cannot be null or empty." });
            }

            try
            {
                await _gameService.CloseGameAsync(gameId, gameCloseDto.WinningNumbers);
                return Ok(new { message = "Game closed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveGameId()
        {
            try
            {
                var activeGame = await _gameService.GetActiveGameAsync();

                if (activeGame == null)
                {
                    return NotFound(new { message = "No active game found." });
                }

                return Ok(new { gameId = activeGame.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = "An error occurred while fetching the active game ID.", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            try
            {
                var games = await _gameService.GetAllGamesAsync();
                return Ok(new { message = "Games retrieved successfully.", data = games });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching games.");
            }
        }

        [HttpGet("closed")]
        public async Task<IActionResult> GetClosedGames()
        {
            try
            {
                var closedGames = await _gameService.GetClosedGamesAsync();
                return Ok(new { message = "Closed games retrieved successfully.", data = closedGames });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = "An error occurred while fetching closed games.", error = ex.Message });
            }
        }
    }
}