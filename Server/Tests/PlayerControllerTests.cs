using System.Net.Http.Json;
using System.Net;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs.PlayerDto;
using Service.DTOs.TransactionDto;

namespace Tests
{
    public class PlayerControllerTests : IClassFixture<ApiTestBase>
    {
        private readonly ApiTestBase _factory;
        private readonly HttpClient _client;

        public PlayerControllerTests(ApiTestBase factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetPlayerDetails_Success()
        {
            // Arrange
            var userManager = _factory.Services.GetService(typeof(UserManager<Player>))
                as UserManager<User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var player = await TestObjects.GetPlayer(userManager);

            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateBalanceRequest = new TransactionCreateDto { Amount = 20m };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/player/{player.Id}/deposit", updateBalanceRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);  // Ensure it returns OK on valid request

            var updatedBalanceResponse = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();
            Assert.NotNull(updatedBalanceResponse);
            Assert.Equal(120m, updatedBalanceResponse.Amount);  // Ensure balance is updated correctly
        }

        

        [Fact]
        public async Task UpdatePlayerBalance_Failure_InvalidAmount()
        {
            // Arrange
            var userManager = _factory.Services.GetService(typeof(UserManager<Player>))
                as UserManager<User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
            using var scope = _factory.Services.CreateScope();  // Create a scope to resolve scoped services like UserManager
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            Assert.NotNull(userManager);  // Ensure userManager is resolved correctly

            var player = await TestObjects.GetPlayer(userManager);
            Assert.NotNull(player);  // Ensure the player was created successfully

            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");
            Assert.NotNull(token);  // Ensure the token is retrieved

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"/api/player/{player.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            var deletedPlayer = await userManager.FindByIdAsync(player.Id.ToString());
            Assert.Null(deletedPlayer);
        }


    }
}
