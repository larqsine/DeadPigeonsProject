using Microsoft.AspNetCore.Mvc;
using Service.DTOs.BoardDto;
using Service.Interfaces;
using System;
using System.Threading.Tasks;
using Service.DTOs.PlayerDto;
using Service.DTOs.UserDto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly IPlayerService _playerService;

        public BoardController(IBoardService boardService, IPlayerService playerService)
        {
            _boardService = boardService;
            _playerService = playerService;
        }

        // POST: api/board/purchase
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseBoard([FromBody] BoardPurchaseDto boardPurchaseDto)
        {
            if (boardPurchaseDto == null)
                return BadRequest("Board purchase data is required.");

            var player = await _playerService.GetPlayerByIdAsync(boardPurchaseDto.PlayerId);
            if (player == null)
                return NotFound("Player not found.");

            if (!player.AnnualFeePaid.HasValue || player.AnnualFeePaid == false)
                return BadRequest("Player is not eligible to purchase boards. Annual fee is not paid.");

            decimal totalCost = 0m;
            foreach (var boardDto in boardPurchaseDto.Boards)
            {
                if (boardDto.FieldsCount < 5 || boardDto.FieldsCount > 8)
                    return BadRequest("FieldsCount must be between 5 and 8.");

                totalCost += CalculateBoardCost(boardDto.FieldsCount);
            }

            if (player.Balance < totalCost)
                return BadRequest("Insufficient balance to purchase boards.");

            foreach (var boardDto in boardPurchaseDto.Boards)
            {
                var board = await _boardService.CreateBoardAsync(boardDto);
                player.Balance -= board.Cost;
            }

            var playerUpdateDto = new PlayerUpdateDto
            {
                Balance = player.Balance
            };

            await _playerService.UpdatePlayerAsync(player.Id, playerUpdateDto);

            return Ok(new { message = "Board(s) purchased successfully." });
        }

        // Calculate board cost based on FieldsCount
        private decimal CalculateBoardCost(int fieldsCount)
        {
            switch (fieldsCount)
            {
                case 5:
                    return 20m;
                case 6:
                    return 40m;
                case 7:
                    return 80m;
                case 8:
                    return 160m;
                default:
                    throw new ArgumentException("FieldsCount must be between 5 and 8.");
            }
        }
    }
}
