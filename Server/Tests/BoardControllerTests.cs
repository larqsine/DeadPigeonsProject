using System.Net.Http.Json;
using System.Net;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using DataAccess.Models;
using Service.DTOs.BoardDto;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using API;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Tests
{
    public class BoardControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public BoardControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task GetBoardsByPlayerId_Success()
        {
        
            var playerId = Guid.NewGuid(); 

          
            var response = await _client.GetAsync($"/api/board/{playerId}/BoardsByPlayerId");
            var result = await response.Content.ReadFromJsonAsync<List<BoardResponseDto>>();

         
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.All(result, board => Assert.Equal(playerId, board.PlayerId));
        }
        
    }
}
