using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;
using Service.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(IPlayerService playerService, ILogger<PlayerController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }
        

        [HttpGet("{playerId:guid}")]
        [Authorize(Policy = "AdminPolicy")] 

        public async Task<ActionResult<PlayerResponseDto>> GetPlayer([FromRoute] Guid playerId)
        {
            try
            {
                var player = await _playerService.GetPlayerByIdAsync(playerId);
                return Ok(player);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Player not found for ID: {PlayerId}", playerId);
                return NotFound("Player not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching player.");
                return StatusCode(500, "An error occurred while retrieving the player.");
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminPolicy")] 

        public async Task<ActionResult<List<PlayerResponseDto>>> GetAllPlayers()
        {
            try
            {
                var players = await _playerService.GetAllPlayersAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all players.");
                return StatusCode(500, "An error occurred while retrieving players.");
            }
        }

        [HttpPut("{playerId:guid}")]
        [Authorize(Policy = "AdminPolicy")] 

        public async Task<ActionResult<PlayerResponseDto>> UpdatePlayer(
            [FromRoute] Guid playerId,
            [FromBody] PlayerUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPlayer = await _playerService.UpdatePlayerAsync(playerId, updateDto);
                return Ok(updatedPlayer);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Player not found for ID: {PlayerId}", playerId);
                return NotFound("Player not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating player.");
                return StatusCode(500, "An error occurred while updating the player.");
            }
        }

        [HttpDelete("{playerId:guid}")]
        [Authorize(Policy = "AdminPolicy")] 

        public async Task<IActionResult> DeletePlayer([FromRoute] Guid playerId)
        {
            try
            {
                await _playerService.DeletePlayerAsync(playerId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Player not found for ID: {PlayerId}", playerId);
                return NotFound("Player not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting player.");
                return StatusCode(500, "An error occurred while deleting the player.");
            }
        }

        [HttpPost("{playerId:guid}/deposit")]
        public async Task<IActionResult> AddBalance(
            [FromRoute] Guid playerId,
            [FromBody] TransactionCreateDto transactionCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _playerService.AddBalanceAsync(playerId, transactionCreateDto);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Player not found for ID: {PlayerId}", playerId);
                return NotFound("Player not found.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid transaction for Player ID: {PlayerId}", playerId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding balance.");
                return StatusCode(500, "An error occurred while adding balance.");
            }
        }
        [HttpPut("{playerId:guid}/toggle-active")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<PlayerResponseDto>> TogglePlayerActiveStatus(
            [FromRoute] Guid playerId,
            [FromBody] bool isActive) // True for active, False for inactive
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPlayer = await _playerService.TogglePlayerActiveStatusAsync(playerId, isActive);
                return Ok(updatedPlayer);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Player not found for ID: {PlayerId}", playerId);
                return NotFound("Player not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling player active status.");
                return StatusCode(500, "An error occurred while updating the player status.");
            }
        }

    }
}
