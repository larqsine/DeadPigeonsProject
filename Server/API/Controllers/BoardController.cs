using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using Service.DTOs.BoardDto;
using System.Security.Claims;
using System;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly ILogger<BoardController> _logger;

        public BoardController(IBoardService boardService, ILogger<BoardController> logger)
        {
            _boardService = boardService;
            _logger = logger;
        }

        [HttpPost("{playerId:guid}/buy")]
        public async Task<IActionResult> BuyBoard(
            [FromRoute] Guid playerId,
            [FromBody] BuyBoardRequestDto buyBoardRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var boardResponse = await _boardService.BuyBoardAsync(playerId, buyBoardRequestDto);

                return Ok(new { message = "Board purchased successfully.", data = boardResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while buying the board.");
                return StatusCode(500, "An error occurred while buying the board.");
            }
        }
        
        [HttpGet("{playerId:guid}/BoardsByPlayerId")]
        public async Task<IActionResult> GetBoardsByPlayerId(Guid playerId)
        {
            var boards = await _boardService.GetBoardsByPlayerIdAsync(playerId);
            return Ok(boards);
        }

        [HttpGet("{gameId:guid}/BoardsByGameId")]
        public async Task<IActionResult> GetBoardsByGameId(Guid gameId)
        {
            try
            {
                var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(playerId))
                {
                    return Unauthorized("User is not logged in.");
                }
                
                var boards = await _boardService.GetBoardsByGameAndPlayerIdAsync(gameId, Guid.Parse(playerId));
                return Ok(boards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching boards for the game.");
                return StatusCode(500, "An error occurred while fetching boards.");
            }
        }
        
        [HttpGet("{playerId:guid}/recent-boards")]
        public async Task<IActionResult> GetRecentBoards(Guid playerId)
        {
            try
            {
                var boards = await _boardService.GetRecentBoardsAsync(playerId);
                return Ok(boards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching recent boards.");
                return StatusCode(500, "An error occurred while fetching boards.");
            }
        }
        
        [HttpGet("my-boards")]
        public async Task<IActionResult> GetMyBoards()
        {
            try
            {
                var playerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(playerId))
                {
                    return Unauthorized("User is not logged in.");
                }
                var boards = await _boardService.GetBoardsByPlayerIdAsync(Guid.Parse(playerId));

                return Ok(boards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching boards for the current player.");
                return StatusCode(500, "An error occurred while fetching boards.");
            }
        }
        [HttpGet("{gameId:guid}/players-summary")]
        public async Task<IActionResult> GetPlayersSummaryForGame(Guid gameId)
        {
            try
            {
                var playersSummary = await _boardService.GetPlayersSummaryForGameAsync(gameId);
                return Ok(playersSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching players summary for game {GameId}.", gameId);
                return StatusCode(500, "An error occurred while fetching the players summary.");
            }
        }
    }
}