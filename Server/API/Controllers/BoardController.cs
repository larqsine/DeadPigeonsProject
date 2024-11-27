using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

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
                var board = await _boardService.BuyBoardAsync(
                    playerId,
                    buyBoardRequestDto.FieldsCount,
                    buyBoardRequestDto.Numbers,
                    buyBoardRequestDto.GameId // Pass GameId from the request body
                );

                return Ok(new { message = "Board purchased successfully.", data = board });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while buying the board.");
                return StatusCode(500, "An error occurred while buying the board.");
            }
        }

    }
}