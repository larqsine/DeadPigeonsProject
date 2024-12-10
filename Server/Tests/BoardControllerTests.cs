using System.Net.Http.Json;
using System.Net;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using ApiInterationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Service.DTOs.BoardDto;
using Service.DTOs.UserDto;

namespace Tests
{
    public class BoardControllerTests : IClassFixture<ApiTestBase>
    {
        private readonly ApiTestBase _factory;
        private readonly HttpClient _client;
        private readonly UserManager<User> _userManager;

        public BoardControllerTests(ApiTestBase factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task BuyBoard_Success()
        {
            // Arrange
            var admin = await TestObjects.GetAdmin(_userManager);
            var player = await TestObjects.GetPlayer(_userManager);
            var game = TestObjects.GetGame(admin);

            var buyBoardRequest = new BuyBoardRequestDto
            {
                FieldsCount = 6,
                Numbers = new List<int> { 5, 15, 23, 36, 42, 58 },
                GameId = game.Id,
                RemainingAutoplayWeeks = 4
            };

            // Authenticate as player
            var loginResponse = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = player.Email,
                Password = "PlayerPassword123!"
            });
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/board/{player.Id}/buy", buyBoardRequest);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Board purchased successfully", result);
        }

        [Fact]
        public async Task GetBoardsByPlayerId_Success()
        {
            // Arrange
            var player = await TestObjects.GetPlayer(_userManager);

            // Act
            var response = await _client.GetAsync($"/api/board/{player.Id}/BoardsByPlayerId");
            var boards = await response.Content.ReadFromJsonAsync<List<Board>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(boards);
        }

        [Fact]
        public async Task GetBoardsByGameId_Success()
        {
            // Arrange
            var admin = await TestObjects.GetAdmin(_userManager);
            var player = await TestObjects.GetPlayer(_userManager);
            var game = TestObjects.GetGame(admin);
            var board = TestObjects.GetBoard(player, game);

            // Add the board (you may need to call the service or directly seed the DB here)
            // Example: Add to in-memory context

            // Act
            var response = await _client.GetAsync($"/api/board/{game.Id}/BoardsbyGameId");
            var boards = await response.Content.ReadFromJsonAsync<List<Board>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(boards);
            Assert.Contains(boards, b => b.Id == board.Id);
        }

        [Fact]
        public async Task BuyBoard_Failure_InvalidData()
        {
            // Arrange
            var player = await TestObjects.GetPlayer(_userManager);

            var invalidBuyBoardRequest = new BuyBoardRequestDto
            {
                FieldsCount = 10, // Invalid: out of range (should be 5-8)
                Numbers = new List<int> { 5, 15 },
                GameId = Guid.NewGuid(),
                RemainingAutoplayWeeks = 4
            };

            // Authenticate as player
            var loginResponse = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = player.Email,
                Password = "PlayerPassword123!"
            });
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultDto>();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Token);

            // Act
            var response = await _client.PostAsJsonAsync($"/api/board/{player.Id}/buy", invalidBuyBoardRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
