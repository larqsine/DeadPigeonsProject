using System.Net;
using System.Net.Http.Json;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs.WinnerDto;

namespace Tests;

public class WinnerControllerTests : IClassFixture<ApiTestBase>
{
    private readonly HttpClient _client;
    private readonly ApiTestBase _factory;

    public WinnerControllerTests(ApiTestBase factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetWinnersByGameId_ReturnsOk_WhenWinnersExist() // failing, needs work
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var client = _factory.CreateClient();
        var winnerRepository = scopedServices.GetRequiredService<WinnerRepository>();
        var gameRepository = scopedServices.GetRequiredService<GameRepository>();
        var playerRepository = scopedServices.GetRequiredService<PlayerRepository>();
        var boardRepository = scopedServices.GetRequiredService<BoardRepository>();

        var game = new Game
        {
            Id = Guid.NewGuid()
        };
        await gameRepository.CreateGameAsync(game);


        var player = new Player
        {
            Id = Guid.NewGuid(),
            UserName = "TestPlayer1",
            FullName = "Test Player One",
            Email = "testplayer1@example.com",
            PhoneNumber = "123-456-7890",
            Balance = 1000m,
            AnnualFeePaid = true,
            CreatedAt = DateTime.UtcNow
        };

        await playerRepository.SaveAsync();


        var board = new Board
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            GameId = game.Id,
            Numbers = "1, 2, 3, 4, 5",
            Autoplay = true,
            FieldsCount = 5,
            Cost = 50m,
            CreatedAt = DateTime.UtcNow,
            IsWinning = true
        };
        await boardRepository.CreateBoardAsync(board);


        var winner1 = new Winner
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            PlayerId = player.Id,
            BoardId = board.Id,
            WinningAmount = 100m
        };
        var winner2 = new Winner
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            PlayerId = player.Id,
            BoardId = board.Id,
            WinningAmount = 150m
        };

        await winnerRepository.AddWinnersAsync(new List<Winner> { winner1, winner2 });


        await scopedServices.GetRequiredService<DBContext>().SaveChangesAsync();


        var response = await client.GetAsync($"/api/winner/games/{game.Id:D}/GetAllWinners");

        response.EnsureSuccessStatusCode();


        var result = await response.Content.ReadFromJsonAsync<List<WinnerDto>>();


        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, winner => Assert.Equal(game.Id, winner.GameId));
    }


    [Fact]
    public async Task GetWinnersByGameId_ReturnsNotFound_WhenNoWinnersExist()
    {
        var gameId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/winner/games/{gameId:D}/GetAllWinners");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var returnValue = await response.Content.ReadAsStringAsync();

        Assert.NotNull(returnValue);

        var message = returnValue.Contains("message") ? returnValue.Substring(returnValue.IndexOf("message")) : null;

        Assert.Contains("No winners found for the specified game.", message);
    }
}