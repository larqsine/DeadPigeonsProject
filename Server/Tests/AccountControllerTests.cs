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

namespace Tests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AccountControllerTests(WebApplicationFactory<Program> factory)
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
        public async Task Register_Admin_Success()
        {
            var userManager = GetUserManager();
            var admin = await TestObjects.GetAdmin(userManager);

            Assert.NotNull(admin);
            Assert.Equal("Admin User", admin.FullName);
        }

        [Fact]
        public async Task Register_Admin_Failure_Unauthorized()
        {
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

           
            var response = await _client.PostAsJsonAsync("/api/account/register", new
            {
                FullName = "New Admin User",
                Email = "admin@example.com",
                Password = "AdminPassword123!"
            });

            
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Login_Player_Success()
        {
          
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            Assert.False(string.IsNullOrEmpty(token), "Player login token is null or empty.");
        }

        [Fact]
        public async Task Login_Player_Failure_InvalidCredentials()
        {
           
            var response = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = "player@example.com",
                Password = "WrongPassword123!"
            });

            var result = await response.Content.ReadAsStringAsync();

        
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains("invalid", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Player_ChangePassword_Success()
        {
          
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

           
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            
            var changePasswordResponse = await _client.PostAsJsonAsync("/api/account/change-password", new
            {
                CurrentPassword = "PlayerPassword123!",
                NewPassword = "NewPlayerPassword123!"
            });

        
            Assert.Equal(HttpStatusCode.OK, changePasswordResponse.StatusCode);
        }

        [Fact]
        public async Task Player_ChangePassword_Failure_WrongCurrentPassword()
        {
           
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        
            var changePasswordResponse = await _client.PostAsJsonAsync("/api/account/change-password", new
            {
                CurrentPassword = "WrongCurrentPassword123!",
                NewPassword = "NewPlayerPassword123!"
            });

            var result = await changePasswordResponse.Content.ReadAsStringAsync();

         
            Assert.Equal(HttpStatusCode.BadRequest, changePasswordResponse.StatusCode);
            Assert.Contains("incorrect", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Admin_Access_Required_Failure()
        {
          
            var userManager = GetUserManager();
            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

         
            var response = await _client.PostAsJsonAsync("/api/account/register", new
            {
                FullName = "New Admin User",
                Email = "admin@example.com",
                Password = "AdminPassword123!"
            });

           
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
