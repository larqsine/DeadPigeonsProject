using System.Net.Http.Json;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Tests;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Service.DTOs.BoardDto;

namespace Tests
{
    public class BoardControllerTests : IClassFixture<ApiTestBase>
    {
        private readonly ApiTestBase _factory;
        private readonly HttpClient _client;

        public BoardControllerTests(ApiTestBase factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        
        [Fact]
        public async Task GetBoardsByPlayerId_Success()
        {
            // Arrange
            var playerId = Guid.NewGuid(); // Replace with seeded player ID if available

            // Act
            var response = await _client.GetAsync($"/api/board/{playerId}/BoardsByPlayerId");
            var result = await response.Content.ReadFromJsonAsync<List<BoardResponseDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.All(result, board => Assert.Equal(playerId, board.PlayerId));
        }

        [Fact]
        public async Task GetBoardsByGameId_Success()
        {
            // Arrange
            var gameId = Guid.NewGuid(); // Replace with seeded game ID if available

            // Act
            var response = await _client.GetAsync($"/api/board/{gameId}/BoardsByGameId");
            var result = await response.Content.ReadFromJsonAsync<List<BoardResponseDto>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.All(result, board => Assert.Equal(gameId, board.GameId));
        }

        [Fact]
        public async Task BuyBoard_Failure_InvalidFields()
        {
            // Fetch or create valid IDs
            var playersResponse = await _client.GetAsync("/api/players");
            var players = await playersResponse.Content.ReadFromJsonAsync<List<Player>>();
            var playerId = players.First().Id;

            var gamesResponse = await _client.GetAsync("/api/games");
            var games = await gamesResponse.Content.ReadFromJsonAsync<List<Game>>();
            var gameId = games.First().Id;

            var buyBoardRequest = new
            {
                GameId = gameId,
                FieldsCount = 10,
                Numbers = "1,2,3,4,5",
                RemainingAutoplayWeeks = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/board/{playerId}/buy", buyBoardRequest);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid number of fields", errorMessage);
        }


    }
}
