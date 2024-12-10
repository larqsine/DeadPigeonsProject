using Microsoft.AspNetCore.Identity;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using Bogus;

namespace ApiInterationTests;

public static class TestObjects
{
    // Admin Test Object with Password
    public static async Task<Admin> GetAdmin(UserManager<User> userManager)
    {
        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            FullName = "Admin User",
            Email = "admin@example.com",
            UserName = "AdminUser",
            CreatedAt = DateTime.UtcNow,
            PasswordChangeRequired = false
        };

        var result = await userManager.CreateAsync(admin, "AdminPassword123!");
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create admin user.");
        }

        return admin;
    }

    // Player Test Object with Password
    public static async Task<Player> GetPlayer(UserManager<User> userManager)
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            FullName = "Test Player",
            Email = "player@example.com",
            UserName = "TestPlayer",
            CreatedAt = DateTime.UtcNow,
            Balance = 100m,
            AnnualFeePaid = true,
            PasswordChangeRequired = true
        };

        var result = await userManager.CreateAsync(player, "PlayerPassword123!");
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create player user.");
        }

        return player;
    }

    // Game Test Object
    public static Game GetGame(Admin admin)
    {
        return new Faker<Game>()
            .RuleFor(g => g.Id, f => Guid.NewGuid())
            .RuleFor(g => g.AdminId, admin.Id)
            .RuleFor(g => g.StartDate, f => DateOnly.FromDateTime(DateTime.UtcNow))
            .RuleFor(g => g.EndDate, f => f.Random.Bool() ? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)) : null)
            .RuleFor(g => g.IsClosed, f => f.Random.Bool())
            .RuleFor(g => g.TotalRevenue, f => f.Finance.Amount())
            .RuleFor(g => g.PrizePool, f => f.Finance.Amount())
            .RuleFor(g => g.CreatedAt, f => DateTime.UtcNow)
            .Generate();
    }

    // Board Test Object
    public static Board GetBoard(Player player, Game game)
    {
        return new Faker<Board>()
            .RuleFor(b => b.Id, f => Guid.NewGuid())
            .RuleFor(b => b.PlayerId, player.Id)
            .RuleFor(b => b.GameId, game.Id)
            .RuleFor(b => b.Numbers, f => string.Join(",", f.Random.Int(1, 100), f.Random.Int(1, 100), f.Random.Int(1, 100)))
            .RuleFor(b => b.FieldsCount, f => f.Random.Int(5, 8))
            .RuleFor(b => b.Cost, f => f.Finance.Amount())
            .RuleFor(b => b.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(b => b.IsWinning, f => f.Random.Bool())
            .RuleFor(b => b.RemainingAutoplayWeeks, f => f.Random.Int(0, 10))
            .RuleFor(b => b.Autoplay, f => f.Random.Bool())
            .Generate();
    }

    // Transaction Test Object
    public static Transaction GetTransaction(Player player)
    {
        return new Faker<Transaction>()
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.PlayerId, player.Id)
            .RuleFor(t => t.Amount, f => f.Finance.Amount())
            .RuleFor(t => t.TransactionType, f => f.Commerce.ProductName())
            .RuleFor(t => t.MobilepayNumber, f => f.Phone.PhoneNumber())
            .RuleFor(t => t.CreatedAt, f => DateTime.UtcNow)
            .Generate();
    }

    // Winner Test Object
    public static Winner GetWinner(Game game, Player player, Board board)
    {
        return new Faker<Winner>()
            .RuleFor(w => w.Id, f => Guid.NewGuid())
            .RuleFor(w => w.GameId, game.Id)
            .RuleFor(w => w.PlayerId, player.Id)
            .RuleFor(w => w.BoardId, board.Id)
            .RuleFor(w => w.WinningAmount, f => f.Finance.Amount())
            .RuleFor(w => w.CreatedAt, f => DateTime.UtcNow)
            .Generate();
    }
}
