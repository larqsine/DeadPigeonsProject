using System.Net.Http.Json;
using System.Net;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

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
            var result = await response.Content.ReadFromJsonAsync<Player>();

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

            // Initialize player
            var player = await TestObjects.GetPlayer(userManager);

            // Get the token for the player
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");
    
            // Check if the token is null or empty and fail the test if it is
            if (string.IsNullOrEmpty(token))
            {
                Assert.Fail("Token was not obtained successfully.");
            }

            // Debugging token by logging
            Console.WriteLine("Generated Token: " + token);

            // Add the token to the Authorization header
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Retrieve the player from the userManager
            var user = await userManager.FindByIdAsync(player.Id.ToString());

            var playerFromDb = user as Player;
            Assert.NotNull(playerFromDb);

            var updateBalanceRequest = new { Amount = 50m };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/player/{player.Id}/update-balance", updateBalanceRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Fetch the updated player data
            var updatedPlayer = await userManager.FindByIdAsync(player.Id.ToString());
            var playerUpdated = updatedPlayer as Player;
            Assert.NotNull(playerUpdated);
            Assert.Equal(150m, playerUpdated.Balance);
        }

        [Fact]
        public async Task GetPlayerDetails_Failure_Unauthorized()
        {
            // Act
            var response = await _client.GetAsync("/api/player/some-id");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePlayerBalance_Failure_InvalidAmount()
        {
            // Arrange
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Player>))
                                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateBalanceRequest = new
            {
                Amount = -100m // Invalid amount
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/player/{player.Id}/update-balance", updateBalanceRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadAsStringAsync();
            Assert.Contains("invalid amount", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeletePlayer_Success()
        {
            // Arrange
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Player>))
                                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.DeleteAsync($"/api/player/{player.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var deletedPlayer = await userManager.FindByIdAsync(player.Id.ToString());
            Assert.Null(deletedPlayer);
        }
    }
}
