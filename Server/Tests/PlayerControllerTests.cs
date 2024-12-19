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
    public class PlayerControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public PlayerControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task GetCurrentPlayer_Success()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/player/current");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPlayer_Success()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/player/{player.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetPlayer_Failure_NotFound()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var nonExistingPlayerId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/player/{nonExistingPlayerId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        

        [Fact]
        public async Task DeletePlayer_Failure_Unauthorized()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var playerToDelete = await TestObjects.GetPlayer(userManager); // Another player to delete
            var response = await _client.DeleteAsync($"/api/player/{playerToDelete.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePlayerBalance_Success()
        {
            var userManager = GetUserManager();
            var admin = await TestObjects.GetAdmin(userManager);
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, admin.Email, "AdminPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateBalanceRequest = new TransactionCreateDto
            {
                Amount = 20m,
                MobilepayNumber = "123456789"
            };

            var response = await _client.PostAsJsonAsync($"/api/player/{player.Id}/deposit", updateBalanceRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdatePlayerBalance_Failure_NotFound()
        {
            var userManager = GetUserManager();
            var admin = await TestObjects.GetAdmin(userManager);
            var token = await TestObjects.GetToken(_client, admin.Email, "AdminPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var nonExistingPlayerId = Guid.NewGuid();
            var updateBalanceRequest = new TransactionCreateDto
            {
                Amount = 20m,
                MobilepayNumber = "123456789"
            };

            var response = await _client.PostAsJsonAsync($"/api/player/{nonExistingPlayerId}/deposit", updateBalanceRequest);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
