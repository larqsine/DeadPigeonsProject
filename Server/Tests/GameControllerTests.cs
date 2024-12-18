using System.Net;
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
              
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DBContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<DBContext>(options => options.UseInMemoryDatabase("TestDb"));
            });
        });
    }

    [Fact]
    public async Task StartGame_ShouldReturnOkAndGameDetails()
    {
     
        var client = _factory.CreateClient();
        var gameCreateDto = new GameCreateDto { StartDate = DateOnly.FromDateTime(DateTime.UtcNow) };

   
        var response = await client.PostAsJsonAsync("/api/games/start", gameCreateDto);

 
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message)); 
        Assert.Equal("Game started successfully.", message.GetString());
    }

    [Fact]
    public async Task GetActiveGame_ShouldReturnActiveGame()
    {
   
        var client = _factory.CreateClient();

      
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

     
        var response = await client.GetAsync("/api/games/active");

      
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ActiveGameResponseDto>();

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.GameId); 
    }
    
    
 [Fact]
public async Task CloseGame_ShouldReturn_BadRequest_WhenTotalRevenueIsZero()
{
    
    var client = _factory.CreateClient();
    var gameId = Guid.NewGuid();

    Admin admin = null;
    using (var scope = _factory.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DBContext>();

       
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

       
        context.Games.Add(new Game
        {
            Id = gameId,
            AdminId = admin.Id,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            IsClosed = false,
            TotalRevenue = 0, 
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();
    }

 
    var gameCloseDto = new GameCloseDto
    {
        WinningNumbers = new List<int> { 1, 2, 3, 4, 5 }
    };

   
    var response = await client.PostAsJsonAsync($"/api/games/{gameId}/close", gameCloseDto);

 
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode); 

    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
    Assert.NotNull(result);
    Assert.True(result.TryGetProperty("message", out var message));
    var possibleMessages = new[] { "Total revenue cannot be zero. Check board costs.", "Game ID mismatch." };
    Assert.Contains(message.GetString(), possibleMessages);
    
    using (var scope = _factory.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DBContext>();
        var closedGame = await context.Games.FindAsync(gameId);
        Assert.NotNull(closedGame);
        Assert.False(closedGame.IsClosed);
    }
}





    [Fact]
    public async Task GetActiveGame_ShouldReturnNotFound_WhenNoActiveGameExists()
    {
       
        var client = _factory.CreateClient();

        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();

       
            var activeGame = await context.Games.FirstOrDefaultAsync(g => (bool)!g.IsClosed);
            if (activeGame != null)
            {
                context.Games.Remove(activeGame);
                await context.SaveChangesAsync();
            }
        }

 
        var response = await client.GetAsync("/api/games/active");

   
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("No active game found.", message.GetString());
    }
    [Fact]
    public async Task CloseGame_ShouldReturnBadRequest_WhenWinningNumbersAreEmpty()
    {
        
        var client = _factory.CreateClient();
        var gameId = Guid.NewGuid();

    
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

   
        var gameCloseDto = new GameCloseDto { WinningNumbers = new List<int>() };

     
        var response = await client.PostAsJsonAsync($"/api/games/{gameId}/close", gameCloseDto);

      
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Winning numbers cannot be null or empty.", message.GetString());
    }
    [Fact]
    public async Task StartGame_ShouldReturnBadRequest_WhenAnotherGameIsActive()
    {
 
        var client = _factory.CreateClient();
        
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

  
        var response = await client.PostAsJsonAsync("/api/games/start", gameCreateDto);

  
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("A game is already active.", message.GetString());
    }
    [Fact]
    public async Task GetAllGames_ShouldReturnGamesList()
    {

        var client = _factory.CreateClient();

        
        Admin admin = null;
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DBContext>();
        
          
            admin = new Admin
            {
                Id = Guid.NewGuid(), 
                FullName = "Test Admin", 
                Email = "admin@example.com"
            };
            context.Admins.Add(admin);
            await context.SaveChangesAsync();

        
            context.Games.AddRange(new List<Game>
            {
                new Game { Id = Guid.NewGuid(), AdminId = admin.Id, StartDate = DateOnly.FromDateTime(DateTime.UtcNow), IsClosed = false, CreatedAt = DateTime.UtcNow },
                new Game { Id = Guid.NewGuid(), AdminId = admin.Id, StartDate = DateOnly.FromDateTime(DateTime.UtcNow), IsClosed = false, CreatedAt = DateTime.UtcNow }
            });
            await context.SaveChangesAsync();
        }

   
        var response = await client.GetAsync("/api/games");

       
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        Assert.Equal("Games retrieved successfully.", message.GetString());

        
        Assert.True(result.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Array);
        var games = data.EnumerateArray().ToList();
        Assert.NotEmpty(games); 
    }



    [Fact]
    public async Task CloseGame_ShouldReturnBadRequest_WhenGameDoesNotExist()
    {
       
        var client = _factory.CreateClient();
        var invalidGameId = Guid.NewGuid();

     
        var gameCloseDto = new GameCloseDto
        {
            WinningNumbers = new List<int> { 1, 2, 3, 4, 5 }
        };

    
        var response = await client.PostAsJsonAsync($"/api/games/{invalidGameId}/close", gameCloseDto);

      
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.NotNull(result);
        Assert.True(result.TryGetProperty("message", out var message));
        
        var possibleMessages = new[] { "No active game found.", "Game ID mismatch." };
        Assert.Contains(message.GetString(), possibleMessages);
    }


}
