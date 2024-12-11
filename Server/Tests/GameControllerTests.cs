using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using API;
using DataAccess;
using DataAccess.Models;
using Service.DTOs.GameDto;

namespace Tests;

public class GamesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GamesControllerTests(WebApplicationFactory<Program> factory)
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
    }

    [Fact]
    public async Task StartGame_ShouldReturnOkAndGameDetails()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gameCreateDto = new GameCreateDto { StartDate = DateOnly.FromDateTime(DateTime.UtcNow) };

        // Act
        var response = await client.PostAsJsonAsync("/api/games/start", gameCreateDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message)); 
        Assert.Equal("Game started successfully.", message.GetString());
    }

    [Fact]
    public async Task GetActiveGame_ShouldReturnActiveGame()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Seed an active game
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
            context.Games.Add(new Game
            {
                Id = Guid.NewGuid(),
                AdminId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsClosed = false,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync("/api/games/active");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ActiveGameResponseDto>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.GameId); // Check if GameId is present and not empty
    }
    [Fact]
    public async Task CloseGame_ShouldReturnOk_WhenGameIsClosed()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gameId = Guid.NewGuid();

        // Seed an active admin and game
        Admin admin = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();

            // Create an admin
            admin = new Admin
            {
                Id = Guid.NewGuid(),
                FullName = "Test Admin",
                Email = "admin@example.com",
                PhoneNumber = "123123",
                UserName = "admin"
            };
            context.Admins.Add(admin);
            await context.SaveChangesAsync();

            // Create a game associated with this admin
            context.Games.Add(new Game
            {
                Id = gameId,
                AdminId = admin.Id,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsClosed = false,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // Seed winning numbers for closure
        var gameCloseDto = new GameCloseDto
        {
            WinningNumbers = new List<int> { 1, 2, 3, 4, 5 }
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/games/{gameId}/close", gameCloseDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Game closed successfully.", message.GetString());

        // Verify the game state
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
            var closedGame = await context.Games.FindAsync(gameId);
            Assert.True(closedGame.IsClosed); // Check if the game is marked as closed
        }
    }

    [Fact]
    public async Task GetActiveGame_ShouldReturnNotFound_WhenNoActiveGameExists()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/games/active");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("No active game found.", message.GetString());
    }
    [Fact]
    public async Task CloseGame_ShouldReturnBadRequest_WhenWinningNumbersAreEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gameId = Guid.NewGuid();

        // Seed an active game
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
            context.Games.Add(new Game
            {
                Id = gameId,
                AdminId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsClosed = false,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        // Seed an empty winning numbers DTO
        var gameCloseDto = new GameCloseDto { WinningNumbers = new List<int>() };

        // Act
        var response = await client.PostAsJsonAsync($"/api/games/{gameId}/close", gameCloseDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Winning numbers cannot be null or empty.", message.GetString());
    }
    [Fact]
    public async Task StartGame_ShouldReturnBadRequest_WhenAnotherGameIsActive()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Seed an active game
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
            context.Games.Add(new Game
            {
                Id = Guid.NewGuid(),
                AdminId = Guid.NewGuid(),
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                IsClosed = false,
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
        }

        var gameCreateDto = new GameCreateDto { StartDate = DateOnly.FromDateTime(DateTime.UtcNow) };

        // Act
        var response = await client.PostAsJsonAsync("/api/games/start", gameCreateDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("A game is already active.", message.GetString());
    }
    [Fact]
    public async Task GetAllGames_ShouldReturnGamesList()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Seed an Admin and Games
        Admin admin = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
        
            // Create an admin
            admin = new Admin
            {
                Id = Guid.NewGuid(), 
                FullName = "Test Admin", 
                Email = "admin@example.com"
            };
            context.Admins.Add(admin);
            await context.SaveChangesAsync();

            // Create games associated with this admin
            context.Games.AddRange(new List<Game>
            {
                new Game { Id = Guid.NewGuid(), AdminId = admin.Id, StartDate = DateOnly.FromDateTime(DateTime.UtcNow), IsClosed = false, CreatedAt = DateTime.UtcNow },
                new Game { Id = Guid.NewGuid(), AdminId = admin.Id, StartDate = DateOnly.FromDateTime(DateTime.UtcNow), IsClosed = false, CreatedAt = DateTime.UtcNow }
            });
            await context.SaveChangesAsync();
        }

        // Act
        var response = await client.GetAsync("/api/games");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Games retrieved successfully.", message.GetString());

        // Check if 'data' is an array and contains games
        Assert.True(result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array);
        var games = data.EnumerateArray().ToList();
        Assert.NotEmpty(games);  // Assert at least one game is returned
    }



    [Fact]
    public async Task CloseGame_ShouldReturnNotFound_WhenGameDoesNotExist()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidGameId = Guid.NewGuid();

        // Seed winning numbers for closure
        var gameCloseDto = new GameCloseDto
        {
            WinningNumbers = new List<int> { 1, 2, 3, 4, 5 }
        };

        // Act
        var response = await client.PostAsJsonAsync($"/api/games/{invalidGameId}/close", gameCloseDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Game not found.", message.GetString());
    }

}
