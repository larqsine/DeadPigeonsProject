using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Service.DTOs.GameDto;
using Service.Interfaces;

namespace Service.Services;

using System.Linq;


public class GameService : IGameService
{
    private readonly GameRepository _gameRepository;

    public GameService(GameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }
    
    public async Task<Game?> GetActiveGameAsync()
    {
        return await _gameRepository.GetActiveGameAsync();
    }

    
    public async Task<GameDetailsDto> StartNewGameAsync()
    {
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame != null)
            throw new Exception("A game is already active.");

        var game = new Game
        {
            Id = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate = null,
            TotalRevenue = 0m,
            PrizePool = 0m,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        var createdGame = await _gameRepository.CreateGameAsync(game);

        return new GameDetailsDto
        {
            Id = createdGame.Id,
            StartDate = createdGame.StartDate,
            EndDate = createdGame.EndDate,
            TotalRevenue = createdGame.TotalRevenue,
            PrizePool = createdGame.PrizePool,
            IsClosed = createdGame.IsClosed.Value,
            WinningNumbers = createdGame.WinningNumbers
        };
    }
    public async Task CloseGameAsync(Guid gameId, List<int> winningNumbers)
    {

        // Validate active game
        var activeGame = await _gameRepository.GetActiveGameAsync();
        if (activeGame == null || activeGame.Id != gameId)
        {
            throw new Exception("No active game found or game mismatch.");
        }

        // Validate winning numbers
        if (winningNumbers == null || winningNumbers.Count == 0)
        {
            throw new Exception("Winning numbers cannot be null or empty.");
        }
        
        // Calculate rollover if prize pool exceeds 5000
        decimal rolloverAmount = activeGame.PrizePool > 5000
            ? activeGame.PrizePool - 5000
            : 0;

        // Update game status in the repository
        await _gameRepository.CloseGameAsync(gameId, winningNumbers, rolloverAmount);

    }



    public bool IsWinningBoard(string boardNumbers, List<int> winningNumbers)
    {
        // Convert the board's string of numbers into a list of integers
        var boardArray = boardNumbers.Split(',').Select(int.Parse).ToList();

        // Check if all the winning numbers are present in the board's numbers
        return winningNumbers.All(boardArray.Contains);
    
    }



}
