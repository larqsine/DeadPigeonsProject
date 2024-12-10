using Microsoft.AspNetCore.Identity;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using Bogus;
using Service.DTOs.UserDto;

namespace Tests;

using System.Collections.Concurrent;

using System.Collections.Concurrent;

public static class TestObjects
{
    private static readonly ConcurrentDictionary<string, object> SeededData = new();

    public static async Task<Admin> GetAdmin(UserManager<User> userManager)
    {
        if (SeededData.TryGetValue(nameof(Admin), out var existingAdmin))
        {
            return (Admin)existingAdmin;
        }

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

        SeededData[nameof(Admin)] = admin;
        return admin;
    }

    public static async Task<Player> GetPlayer(UserManager<User> userManager)
    {
        if (SeededData.TryGetValue(nameof(Player), out var existingPlayer))
        {
            return (Player)existingPlayer;
        }

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

        SeededData[nameof(Player)] = player;
        return player;
    }

    public static async Task<string> GetToken(HttpClient client, string email, string password)
    {
        if (SeededData.TryGetValue($"{email}_Token", out var token))
        {
            return (string)token;
        }

        var response = await client.PostAsJsonAsync("/api/account/login", new
        {
            Email = email,
            Password = password
        });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LoginResultDto>();
        if (result == null || string.IsNullOrEmpty(result.Token))
        {
            throw new Exception("Failed to login and retrieve token.");
        }

        SeededData[$"{email}_Token"] = result.Token;
        return result.Token;
    }
}

