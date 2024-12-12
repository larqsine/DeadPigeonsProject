using DataAccess;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Service.Services;

namespace Tests;

public class TransactionServiceTests
{
    private DBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DBContext(options);
    }

    [Fact]
    public async Task ApproveTransactionAsync_ValidTransaction_UpdatesStatusAndBalance()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        // Seed data
        var transactionId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            PlayerId = playerId,
            Amount = 100m,
            Status = TransactionStatus.Pending,
            TransactionType = "deposit"
        };
        var player = new Player
        {
            Id = playerId,
            Balance = 50m
        };
        dbContext.Transactions.Add(transaction);
        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync();

        var transactionRepository = new TransactionRepository(dbContext);
        var playerRepository = new PlayerRepository(dbContext);
        var service = new TransactionService(transactionRepository, playerRepository);

        // Act
        var result = await service.ApproveTransactionAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TransactionStatus.Approved, transaction.Status);
        Assert.Equal(150m, player.Balance); // Updated balance
    }

    [Fact]
    public async Task ApproveTransactionAsync_TransactionNotFound_ReturnsNull()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        var transactionRepository = new TransactionRepository(dbContext);
        var playerRepository = new PlayerRepository(dbContext);
        var service = new TransactionService(transactionRepository, playerRepository);

        // Act
        var result = await service.ApproveTransactionAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeclineTransactionAsync_ValidTransaction_UpdatesStatus()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        var transactionId = Guid.NewGuid();
        var transaction = new Transaction
        {
            Id = transactionId,
            Status = TransactionStatus.Pending,
            TransactionType = "deposit"
        };
        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync();

        var transactionRepository = new TransactionRepository(dbContext);
        var playerRepository = new PlayerRepository(dbContext);
        var service = new TransactionService(transactionRepository, playerRepository);

        // Act
        var result = await service.DeclineTransactionAsync(transactionId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TransactionStatus.Declined, transaction.Status);
    }

    [Fact]
    public async Task GetTransactionsByPlayerIdAsync_ValidPlayerId_ReturnsTransactionList()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        var playerId = Guid.NewGuid();
        var transactions = new List<Transaction>
        {
            new Transaction { Id = Guid.NewGuid(), PlayerId = playerId,TransactionType = "deposit", Amount = 100m, Status = TransactionStatus.Approved },
            new Transaction { Id = Guid.NewGuid(), PlayerId = playerId,TransactionType = "deposit", Amount = 200m, Status = TransactionStatus.Pending }
        };
        dbContext.Transactions.AddRange(transactions);
        await dbContext.SaveChangesAsync();

        var transactionRepository = new TransactionRepository(dbContext);
        var playerRepository = new PlayerRepository(dbContext);
        var service = new TransactionService(transactionRepository, playerRepository);

        // Act
        var result = await service.GetTransactionsByPlayerIdAsync(playerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(100m, result[0].Amount);
        Assert.Equal(200m, result[1].Amount);
    }

    [Fact]
    public async Task GetTransactionsByPlayerIdAsync_NoTransactions_ReturnsEmptyList()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();

        var transactionRepository = new TransactionRepository(dbContext);
        var playerRepository = new PlayerRepository(dbContext);
        var service = new TransactionService(transactionRepository, playerRepository);

        // Act
        var result = await service.GetTransactionsByPlayerIdAsync(Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}