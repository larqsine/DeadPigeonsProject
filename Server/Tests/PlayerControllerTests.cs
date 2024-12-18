using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DataAccess.Models;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;
using System;
using System.Linq;
using System.Collections.Generic;
using API;
using DataAccess;

namespace Tests
{
    public class PlayerControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PlayerControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DBContext with in-memory database
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
        public async Task GetPlayerDetails_Success()
        {
            // Arrange
            var player = new Player
            {
                Id = Guid.NewGuid(),
                FullName = "Test Player",
                Email = "testplayer@example.com",
                Balance = 100m
            };

            // Seed the player into the in-memory database
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();
                context.Players.Add(player);
                await context.SaveChangesAsync();
            }

            // Act
            var response = await _client.GetAsync($"/api/player/{player.Id}");
            var result = await response.Content.ReadFromJsonAsync<PlayerResponseDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(player.FullName, result.FullName);
        }

        [Fact]
        public async Task UpdatePlayerBalance_Success()
        {
            // Arrange
            var player = new Player
            {
                Id = Guid.NewGuid(),
                FullName = "Test Player",
                Email = "testplayer@example.com",
                Balance = 100m
            };

            // Seed the player into the in-memory database
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();
                context.Players.Add(player);
                await context.SaveChangesAsync();
            }

            var updateBalanceRequest = new TransactionCreateDto { Amount = 20m };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/player/{player.Id}/deposit", updateBalanceRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var updatedBalanceResponse = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();
            Assert.NotNull(updatedBalanceResponse);
            Assert.Equal(120m, updatedBalanceResponse.Amount);
        }

        [Fact]
        public async Task UpdatePlayerBalance_Failure_InvalidAmount()
        {
            // Arrange
            var player = new Player
            {
                Id = Guid.NewGuid(),
                FullName = "Test Player",
                Email = "testplayer@example.com",
                Balance = 100m
            };

            // Seed the player into the in-memory database
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();
                context.Players.Add(player);
                await context.SaveChangesAsync();
            }

            var updateBalanceRequest = new TransactionCreateDto { Amount = -100m };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/player/{player.Id}/deposit", updateBalanceRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid transaction", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeletePlayer_Success()
        {
            // Arrange
            var player = new Player
            {
                Id = Guid.NewGuid(),
                FullName = "Test Player",
                Email = "testplayer@example.com",
                Balance = 100m
            };

            // Seed the player into the in-memory database
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();
                context.Players.Add(player);
                await context.SaveChangesAsync();
            }

            // Act
            var response = await _client.DeleteAsync($"/api/player/{player.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Ensure the player is deleted from the database
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DBContext>();
                var deletedPlayer = await context.Players.FindAsync(player.Id);
                Assert.Null(deletedPlayer);
            }
        }
    }
}
