using Microsoft.AspNetCore.Mvc;
using Service.DTOs.PlayerDto;
using Service.Interfaces;
    
namespace API.Controllers;

[ApiController]
[Route("/api/player")]
public class PlayerController: ControllerBase
{
    private readonly IPlayerService _playerService;


    public PlayerController(IPlayerService playerService)
    {
        _playerService = playerService;
    }
    
    [HttpPost]
    [Route("create")]
    public ActionResult<PlayerResponseDto> Create([FromBody] PlayerCreateDto createDto)
    {
        try
        {
            return Ok(_playerService.CreatePlayer(createDto));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}