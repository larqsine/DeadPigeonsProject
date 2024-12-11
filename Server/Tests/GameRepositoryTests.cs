using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;

namespace Tests;

public class GameRepositoryTests
{
    private readonly DBContext _context;
    private readonly GameRepository _repository;

    public GameRepositoryTests()
    {
        // In-memory database for testing
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DBContext(options);
        _repository = new GameRepository(_context);

        // Adding initial test data to the in-memory database
        var adminId = Guid.NewGuid();
        _context.Games.AddRange(new List<Game>
        {
            new Game
            {
                Id = Guid.NewGuid(),
                AdminId = adminId,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
                TotalRevenue = 500m,
                PrizePool = 300m,
                IsClosed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new Game
            {
                Id = Guid.NewGuid(),
                AdminId = adminId,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalRevenue = 0,
                PrizePool = 0,
                IsClosed = false,
                CreatedAt = DateTime.UtcNow
            }
        });

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetGameByIdAsync_ShouldReturnGame_WhenExists()
    {
        // Arrange
        var gameId = _context.Games.First(g => g.IsClosed==false).Id;

        // Act
        var result = await _repository.GetGameByIdAsync(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameId, result?.Id);
        Assert.False(result?.IsClosed);
    }

    [Fact]
    public async Task GetGameByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var nonExistentGameId = Guid.NewGuid();

        // Act
        var result = await _repository.GetGameByIdAsync(nonExistentGameId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveGameAsync_ShouldReturnActiveGame()
    {
        // Act
        var result = await _repository.GetActiveGameAsync();

        // Assert
        Assert.NotNull(result);
        Assert.False(result?.IsClosed);
    }

    [Fact]
    public async Task GetActiveGameAsync_ShouldReturnNull_WhenNoActiveGame()
    {
        // Arrange
        foreach (var game in _context.Games)
        {
            game.IsClosed = true;
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveGameAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateGameAsync_ShouldAddGameToDatabase()
    {
        // Arrange
        var newGame = new Game
        {
            Id = Guid.NewGuid(),
            AdminId = Guid.NewGuid(),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            TotalRevenue = 0,
            PrizePool = 0,
            IsClosed = false,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var createdGame = await _repository.CreateGameAsync(newGame);

        // Assert
        Assert.NotNull(createdGame);
        Assert.Equal(newGame.Id, createdGame.Id);
        Assert.False(createdGame.IsClosed);
    }

    [Fact]
    public async Task CloseGameAsync_ShouldMarkGameAsClosed()
    {
        // Arrange
        var activeGame = _context.Games.First(g => g.IsClosed==false);
        var winningNumbers = new List<int> { 1, 2, 3, 4, 5 };
        var rolloverAmount = 50m;

        // Act
        await _repository.CloseGameAsync(activeGame.Id, winningNumbers, rolloverAmount);
        var closedGame = await _repository.GetGameByIdAsync(activeGame.Id);

        // Assert
        Assert.NotNull(closedGame);
        Assert.True(closedGame?.IsClosed);
        Assert.Equal(winningNumbers, closedGame?.WinningNumbers);
        Assert.Equal(rolloverAmount, closedGame?.RolloverAmount);
    }

    [Fact]
    public async Task UpdateGameAsync_ShouldModifyGameDetails()
    {
        // Arrange
        var gameToUpdate = _context.Games.First();
        gameToUpdate.TotalRevenue = 1000m;
        gameToUpdate.PrizePool = 800m;

        // Act
        await _repository.UpdateGameAsync(gameToUpdate);
        var updatedGame = await _repository.GetGameByIdAsync(gameToUpdate.Id);

        // Assert
        Assert.NotNull(updatedGame);
        Assert.Equal(1000m, updatedGame?.TotalRevenue);
        Assert.Equal(800m, updatedGame?.PrizePool);
    }
}
