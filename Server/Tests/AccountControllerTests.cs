using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Tests;

namespace Tests
{
    public class AccountControllerTests : IClassFixture<ApiTestBase>
    {
        private readonly ApiTestBase _factory;
        private readonly HttpClient _client;

        public AccountControllerTests(ApiTestBase factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Register_Admin_Success()
        {
            // Use the seeded admin to validate it already exists and works
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Admin>)) 
                                as UserManager<DataAccess.Models.User>;

            var admin = await TestObjects.GetAdmin(userManager);

            Assert.NotNull(admin);
            Assert.Equal("Admin User", admin.FullName);
        }

        [Fact]
        public async Task Register_Admin_Failure_Unauthorized()
        {
            // Arrange: Use a non-admin player token for access
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Player>))
                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            // Add the token to the Authorization header for a non-admin user
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act: Try to access the admin-only register endpoint
            var response = await _client.PostAsJsonAsync("/api/account/register", new
            {
                FullName = "New Admin User",
                Email = "admin@example.com",
                Password = "AdminPassword123!"
            });

            // Assert: Expect Unauthorized (403) for non-admin user
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Login_Player_Success()
        {
            // Use seeded player data and validate login
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Admin>)) 
                                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            Assert.False(string.IsNullOrEmpty(token), "Player login token is null or empty.");
        }

        [Fact]
        public async Task Login_Player_Failure_InvalidCredentials()
        {
            // Arrange: Use invalid credentials
            var response = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = "player@example.com",
                Password = "WrongPassword123!"
            });

            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains("invalid", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Player_ChangePassword_Success()
        {
            // Arrange: Use player and login to get a token
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Player>)) 
                                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            // Add the token to the Authorization header
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act: Change the password
            var changePasswordResponse = await _client.PostAsJsonAsync("/api/account/change-password", new
            {
                CurrentPassword = "PlayerPassword123!",
                NewPassword = "NewPlayerPassword123!"
            });

            // Assert
            Assert.Equal(HttpStatusCode.OK, changePasswordResponse.StatusCode);
        }

        [Fact]
        public async Task Player_ChangePassword_Failure_WrongCurrentPassword()
        {
            // Arrange: Use player and login to get a token
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Player>)) 
                                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act: Try changing password with incorrect current password
            var changePasswordResponse = await _client.PostAsJsonAsync("/api/account/change-password", new
            {
                CurrentPassword = "WrongCurrentPassword123!",
                NewPassword = "NewPlayerPassword123!"
            });

            var result = await changePasswordResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, changePasswordResponse.StatusCode);
            Assert.Contains("incorrect", result, StringComparison.OrdinalIgnoreCase);
        }

       


        

        
        [Fact]
        public async Task Admin_Access_Required_Failure()
        {
            // Arrange: Non-admin player trying to access admin-required endpoint
            var userManager = _factory.Services.GetService(typeof(UserManager<DataAccess.Models.Player>))
                as UserManager<DataAccess.Models.User>;

            var player = await TestObjects.GetPlayer(userManager);
            var token = await TestObjects.GetToken(_client, player.Email, "PlayerPassword123!");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act: Try to access admin-only register endpoint
            var response = await _client.PostAsJsonAsync("/api/account/register", new
            {
                FullName = "New Admin User",
                Email = "admin@example.com",
                Password = "AdminPassword123!"
            });

            // Assert: Unauthorized or Forbidden response (403)
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
