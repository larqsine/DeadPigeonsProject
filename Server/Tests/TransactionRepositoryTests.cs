using DataAccess;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class TransactionRepositoryTests
{
    private readonly DBContext _context;
    private readonly TransactionRepository _repository;

    public TransactionRepositoryTests()
    {
    
        var options = new DbContextOptionsBuilder<DBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _context = new DBContext(options);
        _repository = new TransactionRepository(_context);

       
        _context.Transactions.AddRange(new List<Transaction>
        {
            new Transaction 
            { 
                Id = Guid.NewGuid(), 
                PlayerId = Guid.NewGuid(), 
                Amount = 100m, 
                TransactionType = "deposit", 
                Status = TransactionStatus.Pending
            },
            new Transaction 
            { 
                Id = Guid.NewGuid(), 
                PlayerId = Guid.NewGuid(), 
                Amount = 50m, 
                TransactionType = "purchase", 
                Status = TransactionStatus.Completed
            }
        });

        _context.SaveChanges();
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnTransaction_WhenExists()
    {
        
        var transactionId = _context.Transactions.First(t => t.TransactionType == "deposit").Id;

       
        var result = await _repository.GetTransactionByIdAsync(transactionId);

   
        Assert.NotNull(result);
        Assert.Equal(transactionId, result?.Id);
        Assert.Equal("deposit", result?.TransactionType);
    }

    [Fact]
    public async Task GetTransactionByIdAsync_ShouldReturnNull_WhenNotFound()
    {
      
        var nonExistentTransactionId = Guid.NewGuid();
        
        var result = await _repository.GetTransactionByIdAsync(nonExistentTransactionId);

       
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTransactionsByPlayerIdAsync_ShouldReturnTransactions_ForPlayer()
    {
       
        var playerId = _context.Transactions.First().PlayerId;

     
        var result = await _repository.GetTransactionsByPlayerIdAsync(playerId);

       
        Assert.NotEmpty(result);
        Assert.All(result, t => Assert.Equal(playerId, t.PlayerId));
    }

    [Fact]
    public async Task GetTransactionsByPlayerIdAsync_ShouldReturnEmptyList_WhenNoTransactionsFound()
    {
       
        var nonExistentPlayerId = Guid.NewGuid();

      
        var result = await _repository.GetTransactionsByPlayerIdAsync(nonExistentPlayerId);

      
        Assert.Empty(result);
    }

    [Fact]
    public async Task SaveAsync_ShouldPersistChanges()
    {
     
        var newTransaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PlayerId = Guid.NewGuid(),
            Amount = 30m,
            TransactionType = "purchase", 
            Status = TransactionStatus.Completed
        };

        _context.Transactions.Add(newTransaction);

        
        await _repository.SaveAsync();
        
        var savedTransaction = await _context.Transactions.FindAsync(newTransaction.Id);
        Assert.NotNull(savedTransaction);
        Assert.Equal(newTransaction.Amount, savedTransaction?.Amount);
        Assert.Equal("purchase", savedTransaction?.TransactionType);
        Assert.Equal(TransactionStatus.Completed, savedTransaction?.Status);
    }
}