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