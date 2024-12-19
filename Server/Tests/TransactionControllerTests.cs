using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using API;
using DataAccess;
using Service.DTOs.TransactionDto;

namespace Tests
{
    public class TransactionControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public TransactionControllerTests(WebApplicationFactory<Program> factory)
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
            _client = _factory.CreateClient();
        }

        private UserManager<DataAccess.Models.User> GetUserManager()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<UserManager<DataAccess.Models.User>>();
        }
        

        [Fact]
        public async Task ApproveTransaction_Failure_Unauthorized()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var transactionId = Guid.NewGuid(); // Replace with a valid transaction ID
            var response = await _client.PutAsJsonAsync($"/api/transaction/{transactionId}/approve", new { });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        
        [Fact]
        public async Task DeclineTransaction_Failure_Unauthorized()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var transactionId = Guid.NewGuid(); // Replace with a valid transaction ID
            var response = await _client.PutAsJsonAsync($"/api/transaction/{transactionId}/decline", new { });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetDepositTransactions_Success()
        {
            var response = await _client.GetAsync("/api/transaction/deposit");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetTransactionsByPlayerId_Success()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync($"/api/transaction/{player.Id}/GetPlayerTransaction", new { });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        
    }
}
