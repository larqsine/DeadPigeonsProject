using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DataAccess.Enums;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using API;
using DataAccess;
using DataAccess.Repositories;
using Service.DTOs.TransactionDto;

namespace Tests
{
    public class TransactionControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public TransactionControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DBContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<DBContext>(options => options.UseInMemoryDatabase("TestDb"));
                });
            });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task ApproveTransaction_ReturnsOk_WhenTransactionIsApproved()
        {
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<DBContext>();
            var transactionRepository = new TransactionRepository(context);
            
            var admin = new User { Email = "admin@example.com", UserName = "admin" };
            var player = new User { Email = "player@example.com", UserName = "player" };

            context.Users.Add(admin);
            context.Users.Add(player);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerId = player.Id,
                Amount = 100.0m,
                Status = TransactionStatus.Pending,
                TransactionType = "deposit"
            };

            await transactionRepository.AddTransactionAsync(transaction);
            
            var response = await _client.PutAsJsonAsync<object>($"/api/transaction/{transaction.Id}/approve", null!);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ApproveTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            var transactionId = Guid.NewGuid();

            var response = await _client.PutAsync($"/api/transaction/{transactionId}/approve", null);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeclineTransaction_ReturnsOk_WhenTransactionIsDeclined()
        {
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<DBContext>();
            var transactionRepository = new TransactionRepository(context);
            
            var admin = new User { Email = "admin@example.com", UserName = "admin" };
            var player = new User { Email = "player@example.com", UserName = "player" };

            context.Users.Add(admin);
            context.Users.Add(player);
            await context.SaveChangesAsync();

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerId = player.Id,
                Amount = 50.0m,
                Status = TransactionStatus.Pending,
                TransactionType = "deposit"
            };

            await transactionRepository.AddTransactionAsync(transaction);

            
            var response = await _client.PutAsync($"/api/transaction/{transaction.Id}/decline", null);

           
            var result = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(TransactionStatus.Declined, result.Status);
        }

        [Fact]
        public async Task DeclineTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
        {
            var transactionId = Guid.NewGuid();
            var response = await _client.PutAsync($"/api/transaction/{transactionId}/decline", null);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetTransactionsByPlayerId_Success()
        {
            using var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;

            var context = scopedServices.GetRequiredService<DBContext>();
            var transactionRepository = new TransactionRepository(context);
            
            var admin = new User { Email = "admin@example.com", UserName = "admin" };
            var player = new User { Email = "player@example.com", UserName = "player" };

            context.Users.Add(admin);
            context.Users.Add(player);
            await context.SaveChangesAsync();

       
            var transaction1 = new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerId = player.Id,
                Amount = 100.0m,
                Status = TransactionStatus.Pending,
                TransactionType = "deposit"
            };

            var transaction2 = new Transaction
            {
                Id = Guid.NewGuid(),
                PlayerId = player.Id,
                Amount = 200.0m,
                Status = TransactionStatus.Approved,
                TransactionType = "deposit"
            };

            await transactionRepository.AddTransactionAsync(transaction1);
            await transactionRepository.AddTransactionAsync(transaction2);

          
            var response = await _client.PutAsJsonAsync<object>($"/api/transaction/{player.Id}/GetPlayerTransaction", null!);

         
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<TransactionResponseDto>>();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, transaction => Assert.Equal(player.Id, transaction.PlayerId));
        }
    }
}
