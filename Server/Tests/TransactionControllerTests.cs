using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs.TransactionDto;

namespace Tests;

public class TransactionControllerTests(ApiTestBase factory) : IClassFixture<ApiTestBase>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });

    [Fact]
    public async Task ApproveTransaction_ReturnsOk_WhenTransactionIsApproved()
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var client = factory.CreateClient();
        var userManager = scopedServices.GetRequiredService<UserManager<User>>();
        var transactionRepository = scopedServices.GetRequiredService<TransactionRepository>();

        var admin = await TestObjects.GetAdmin(userManager);
        var player = await TestObjects.GetPlayer(userManager);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            Amount = 100.0m,
            Status = TransactionStatus.Pending,
            TransactionType = "deposit"
        };

        await transactionRepository.AddTransactionAsync(transaction);


        if (admin.Email != null)
        {
            var token = await TestObjects.GetToken(client, admin.Email, "AdminPassword123!");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.PutAsJsonAsync<object>($"/api/transaction/{transaction.Id}/approve", null!);

        response.EnsureSuccessStatusCode();
    }


    [Fact]
    public async Task ApproveTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        var transactionId = Guid.NewGuid();

        var response = await _client.PutAsync($"/api/transaction/{transactionId}/approve", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeclineTransaction_ReturnsOk_WhenTransactionIsDeclined()
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var client = factory.CreateClient();
        var transactionRepository = scopedServices.GetRequiredService<TransactionRepository>();
        var userManager = scopedServices.GetRequiredService<UserManager<User>>();

        var admin = await TestObjects.GetAdmin(userManager);
        var player = await TestObjects.GetPlayer(userManager);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            Amount = 50.0m,
            Status = TransactionStatus.Pending,
            TransactionType = "deposit"
        };

        await transactionRepository.AddTransactionAsync(transaction);


        if (admin.Email != null)
        {
            var token = await TestObjects.GetToken(client, admin.Email, "AdminPassword123!");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


        var response = await client.PutAsync($"/api/transaction/{transaction.Id}/decline", null);


        var result = await response.Content.ReadFromJsonAsync<TransactionResponseDto>();


        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(TransactionStatus.Declined, result.Status);
    }

    [Fact]
    public async Task DeclineTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        var transactionId = Guid.NewGuid();
        var response = await _client.PutAsync($"/api/transaction/{transactionId}/decline", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactionsByPlayerId_Success()
    {
        using var scope = factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        var client = factory.CreateClient();
        var userManager = scopedServices.GetRequiredService<UserManager<User>>();
        var transactionRepository = scopedServices.GetRequiredService<TransactionRepository>();


        var admin = await TestObjects.GetAdmin(userManager);
        var player = await TestObjects.GetPlayer(userManager);


        var transaction1 = new Transaction
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            Amount = 100.0m,
            Status = TransactionStatus.Pending,
            TransactionType = "deposit"
        };

        var transaction2 = new Transaction
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            Amount = 200.0m,
            Status = TransactionStatus.Approved,
            TransactionType = "deposit"
        };

        await transactionRepository.AddTransactionAsync(transaction1);
        await transactionRepository.AddTransactionAsync(transaction2);


        if (admin.Email != null)
        {
            var token = await TestObjects.GetToken(client, admin.Email, "AdminPassword123!");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await client.PutAsJsonAsync<object>($"/api/transaction/{player.Id}/GetPlayerTransaction", null!);


        response.EnsureSuccessStatusCode();


        var result = await response.Content.ReadFromJsonAsync<List<TransactionResponseDto>>();


        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, transaction => Assert.Equal(player.Id, transaction.PlayerId));
    }
}