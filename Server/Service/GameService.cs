using DataAccess;
using DataAccess.Models;

namespace Service;

using System.Linq;

public class GameService
{
    private readonly DBContext _dbContext;

    public GameService(DBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public void ProcessGameResults(int gameId)
    {
        // Fetch the game based on its ID and check if it's closed
        var game = _dbContext.Games.FirstOrDefault(g => g.Id == gameId && g.IsClosed != true);
    
        // If game is not found or winning numbers are not set, throw an error
        if (game == null || game.WinningNumbers == null)
            throw new InvalidOperationException("Game not found or winning numbers not set.");

        // Get the list of boards associated with the game
        var boards = _dbContext.Boards.Where(b => b.GameId == gameId).ToList();

        // Find boards that are winning
        var winningBoards = boards
            .Where(b => IsWinningBoard(b.Numbers, game.WinningNumbers))
            .ToList();

        // If no winning boards, apply rollover logic
        if (!winningBoards.Any())
        {
            game.RolloverAmount += game.PrizePool;
            game.IsClosed = true;
            _dbContext.SaveChanges();
            return;
        }

        // Calculate the prize per winner and create winners
        var totalWinners = winningBoards.Count;
        var prizePerWinner = game.PrizePool / totalWinners;

        foreach (var board in winningBoards)
        {
            var winner = new Winner
            {
                GameId = game.Id,
                PlayerId = board.PlayerId,
                BoardId = board.Id,
                WinningAmount = prizePerWinner,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Winners.Add(winner);
        }

        // Close the game and reset the rollover amount
        game.IsClosed = true;
        game.RolloverAmount = 0;
        _dbContext.SaveChanges();
    }

    public bool IsWinningBoard(string boardNumbers, List<int> winningNumbers)
    {
        // Convert the board's string of numbers into a list of integers
        var boardArray = boardNumbers.Split(',').Select(int.Parse).ToList();

        // Check if all the winning numbers are present in the board's numbers
        return winningNumbers.All(boardArray.Contains);
    
    }



}
