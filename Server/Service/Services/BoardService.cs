using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Service.DTOs.BoardDto;
using Service.DTOs.TransactionDto;
using Service.Interfaces;

namespace Service.Services;

public class BoardService : IBoardService
{
    
        private readonly BoardRepository _boardRepository;
        private readonly PlayerRepository _playerRepository;
        private readonly GameRepository _gameRepository;
        private readonly ILogger<BoardService> _logger;


        public BoardService(
            BoardRepository boardRepository,
            PlayerRepository playerRepository,
            GameRepository gameRepository,
            ILogger<BoardService> logger)
        {
            _boardRepository = boardRepository;
            _playerRepository = playerRepository;
            _gameRepository = gameRepository;
            _logger=logger;
        }
        
        public async Task<BoardResponseDto> BuyBoardAsync(Guid playerId, BuyBoardRequestDto buyBoardRequestDto)
        {
            // Get the player
            var player = await _playerRepository.GetPlayerByIdAsync(playerId);
            if (player == null)
                throw new Exception("Player not found or inactive.");
            
            if(player.AnnualFeePaid== false)
                throw new Exception("Cannot buy a board if annual fee has not been paid.");
            
            // Get current time in Danish time zone
            var denmarkTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
            var currentTimeInDenmark = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, denmarkTimeZone);

            // Check if it's past the participation window (Saturday 5 PM Danish time)
            var isPastDeadline = currentTimeInDenmark.DayOfWeek == DayOfWeek.Saturday && currentTimeInDenmark.Hour >= 17;

            if (isPastDeadline)
            {
                throw new Exception("You cannot participate after Saturday 5 PM Danish time.");
            }

            // Get the game by GameId
            var game = await _gameRepository.GetGameByIdAsync(buyBoardRequestDto.GameId);
            if (game == null)
                throw new Exception("Game not found.");

            if (game.IsClosed == true)
                throw new Exception("Cannot purchase a board for a closed game.");

            // Calculate the cost based on FieldsCount from the DTO
            decimal cost = buyBoardRequestDto.FieldsCount switch
            {
                5 => 20m,
                6 => 40m,
                7 => 80m,
                8 => 160m,
                _ => throw new Exception("Invalid number of fields.")
            };

            // Check if the player has sufficient balance
            if (player.Balance < cost)
                throw new Exception("Insufficient balance.");
            
            

            // Create a purchase transaction
            var purchaseTransactionDto = new TransactionCreateDto
            {
                Amount = cost
            };

            var transactionId = Guid.NewGuid();
            var purchaseTransaction = purchaseTransactionDto.ToPurchaseTransaction(playerId, transactionId);

            // Add the purchase transaction (Transaction Repository handles persisting and deducting balance)
            await _playerRepository.AddTransactionAsync(purchaseTransaction);

            // Use the ToBoard method from the DTO to map to the Board entity
            var board = buyBoardRequestDto.ToBoard(playerId, cost);
            
            if (buyBoardRequestDto.RemainingAutoplayWeeks > 0)
            {
                board.Autoplay = true;
                board.RemainingAutoplayWeeks = buyBoardRequestDto.RemainingAutoplayWeeks;
            }

            // Save the board to the database
            var createdBoard = await _boardRepository.CreateBoardAsync(board);

            // Return the BoardResponseDto with the board details
            return new BoardResponseDto
            {
                Id = createdBoard.Id,
                PlayerId = createdBoard.PlayerId,
                GameId = createdBoard.GameId,
                Numbers = createdBoard.Numbers,
                FieldsCount = createdBoard.FieldsCount,
                Cost = createdBoard.Cost,
                CreatedAt = createdBoard.CreatedAt,
                IsWinning = createdBoard.IsWinning
            };
        }
        public async Task HandleAutoplayAsync(Guid gameId)
{
    try
    {
        // Fetch only the boards that need to be processed (autoplay and remaining weeks > 0)
        var autoplayBoards = await _boardRepository.GetBoardsForAutoplayAsync();

        // Log how many autoplay boards were found
        _logger.LogInformation($"Fetched {autoplayBoards.Count} autoplay boards.");

        if (!autoplayBoards.Any())
        {
            _logger.LogInformation("No autoplay boards found.");
            return;
        }

        foreach (var board in autoplayBoards)
        {
            // Fetch the player associated with the board
            var player = await _playerRepository.GetPlayerByIdAsync(board.PlayerId);
            if (player == null)
            {
                _logger.LogError($"Player {board.PlayerId} not found. Skipping autoplay.");
                continue;
            }

            // Check if the player has enough balance to pay for a new board
            if (player.Balance < board.Cost)
            {
                _logger.LogError($"Player {player.Id} has insufficient balance. Skipping autoplay.");
                continue;
            }

            // Deduct the cost of the board from the player's balance
            player.Balance -= board.Cost;

            // Create a new autoplay board
            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                PlayerId = board.PlayerId,
                GameId = gameId,
                Numbers = board.Numbers,  // Reusing the same numbers for the new board
                FieldsCount = board.FieldsCount,
                Cost = board.Cost,
                CreatedAt = DateTime.UtcNow,
                IsWinning = false,  // Initially set to false
                Autoplay = true,
                RemainingAutoplayWeeks = board.RemainingAutoplayWeeks - 1
            };

            _logger.LogInformation($"Creating new autoplay board for Player {player.Id}, Game {gameId}, Board ID: {newBoard.Id}");

            // Save the new board to the database
            await _boardRepository.CreateBoardAsync(newBoard);

            // Update the player's balance in the database
            await _playerRepository.UpdatePlayerAsync(player);

            // If no weeks remain for autoplay, disable autoplay on the original board
            if (newBoard.RemainingAutoplayWeeks == 0)
            {
                board.Autoplay = false;
                await _boardRepository.UpdateBoardsAsync(new List<Board> { board });
                _logger.LogInformation($"Autoplay disabled for board ID {board.Id} as no weeks remain.");
            }
        }
    }
    catch (Exception ex)
    {
        _logger.LogError($"Error in HandleAutoplayAsync: {ex.Message}");
    }
}

}

        