using Microsoft.AspNetCore.Mvc;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;
using Service.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("/api/player")]
public class PlayerController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<PlayerResponseDto>> Create([FromBody] PlayerCreateDto createDto)
    {
        try
        {
            var player = await _playerService.CreatePlayerAsync(createDto);
            return Ok(player);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{playerId}/deposit")]
    public async Task<IActionResult> AddBalance(
        [FromRoute] Guid playerId, 
        [FromBody] TransactionCreateDto transactionCreateDto)
    {
        Console.WriteLine($"Received playerId: {playerId}");

        if (transactionCreateDto == null)
        {
            return BadRequest("Transaction details are required.");
        }

        if (transactionCreateDto.Amount <= 0)
        {
            return BadRequest("The amount to add must be greater than zero.");
        }

        try
        {
            var response = await _playerService.AddBalanceAsync(playerId, transactionCreateDto);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Player not found.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log the exception details for debugging
            Console.WriteLine($"An error occurred: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, "An error occurred while adding balance: " + ex.Message);
        }
    }

}