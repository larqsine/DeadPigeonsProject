using System.Net;
using System.Net.Http.Json;
using DataAccess;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs.WinnerDto;
using System.Linq;
using API;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class WinnerControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public WinnerControllerTests(WebApplicationFactory<Program> factory)
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

        private DbContext GetDbContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<DBContext>();
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
}
