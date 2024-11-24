using Microsoft.AspNetCore.Mvc;
using Service.DTOs.TransactionDto;
using Service.DTOs.UserDto;
using Service.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("/api/player")]
public class PlayerController(IPlayerService playerService) : ControllerBase
{
    [HttpPost]
    [Route("create")]
    public ActionResult<PlayerResponseDto> Create([FromBody] PlayerCreateDto createDto)
    {
        try
        {
            return Ok(playerService.CreatePlayer(createDto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpPost("{playerId}/deposit")]
    public IActionResult AddBalance(Guid playerId, [FromBody] TransactionCreateDto transactionCreateDto, [FromQuery] decimal amount, [FromQuery] string mobilePayNumber)
    {
        if (transactionCreateDto == null)
        {
            return BadRequest("Transaction details are required.");
        }

        if (amount <= 0)
        {
            return BadRequest("The amount to add must be greater than zero.");
        }

        try
        {
            var response = playerService.AddBalance(playerId, transactionCreateDto, amount, mobilePayNumber);
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
            return StatusCode(500, "An error occurred while adding balance: " + ex.Message);
        }
    }
}