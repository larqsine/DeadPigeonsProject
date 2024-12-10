using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using ApiInterationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Service.DTOs.UserDto;

namespace Tests
{
    public class AccountControllerTests : IClassFixture<ApiTestBase>
    {
        private readonly ApiTestBase _factory;
        private readonly HttpClient _client;

        public AccountControllerTests(
            ApiTestBase factory)
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
            // Arrange
            var newAdmin = new
            {
                Id = new Guid(),
                UserName = "AdminUser2",
                Email = "admin@examplee.com",
                PhoneNumber = "0123456789",
                Fullname = "Admin User",
                Password = "AdminPassword123!",
                Role = "Admin",
                PasswordChangeRequired = false
            };

            // Act - Register
            var response = await _client.PostAsJsonAsync("/api/account/register", newAdmin);
            var jsonString = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(jsonString).RootElement;

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.TryGetProperty("userId", out var userId), "Response does not contain 'userId'.");
            Assert.NotNull(userId.GetString()); // Ensure the userId is not null or empty
        }


        [Fact]
        public async Task Login_Player_Success()
        {
            var response = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = "player@example.com",
                Password = "PlayerPassword123!"
            });

            var result = await response.Content.ReadFromJsonAsync<LoginResultDto>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result?.Token);
        }

        [Fact]
        public async Task ChangePassword_Success()
        {
            // Arrange: Log in as the player
            var loginResponse = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = "player@example.com",
                Password = "PlayerPassword123!"
            });

            var loginJsonString = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonDocument.Parse(loginJsonString).RootElement;

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
            Assert.True(loginResult.TryGetProperty("token", out var token), "Response does not contain 'token'.");
            Assert.False(string.IsNullOrEmpty(token.GetString()), "Token is null or empty.");

            // Add the token to the Authorization header
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.GetString());

            // Act: Change the password
            var changePasswordResponse = await _client.PostAsJsonAsync("/api/account/change-password", new
            {
                CurrentPassword = "PlayerPassword123!",
                NewPassword = "NewPlayerPassword123!"
            });

            // Assert: Check the response
            Assert.Equal(HttpStatusCode.OK, changePasswordResponse.StatusCode);
        }

        [Fact]
        public async Task Register_Admin_Failure_MissingFields()
        {
            // Arrange
            var incompleteAdmin = new
            {
                Email = "admin@incomplete.com",
                Password = "AdminPassword123!" // Missing UserName, Fullname, etc.
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/register", incompleteAdmin);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("required", result,
                StringComparison.OrdinalIgnoreCase); // Ensure error message mentions required fields
        }

        [Fact]
        public async Task Login_Player_Failure_InvalidCredentials()
        {
            // Arrange
            var invalidLogin = new
            {
                Email = "player@example.com",
                Password = "WrongPassword123!" // Incorrect password
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/account/login", invalidLogin);
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains("invalid", result,
                StringComparison.OrdinalIgnoreCase); // Ensure error message mentions invalid credentials
        }

        [Fact]
        public async Task ChangePassword_Failure_WrongCurrentPassword()
        {
            // Arrange: Log in as the player
            var loginResponse = await _client.PostAsJsonAsync("/api/account/login", new
            {
                Email = "player@example.com",
                Password = "PlayerPassword123!"
            });

            var loginJsonString = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonDocument.Parse(loginJsonString).RootElement;
            Assert.True(loginResult.TryGetProperty("token", out var token), "Login response does not contain 'token'.");

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.GetString());

            // Act: Try changing password with incorrect current password
            var changePasswordResponse = await _client.PostAsJsonAsync("/api/account/change-password", new
            {
                CurrentPassword = "WrongCurrentPassword123!", // Incorrect current password
                NewPassword = "NewPlayerPassword123!"
            });

            var result = await changePasswordResponse.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, changePasswordResponse.StatusCode);
            Assert.Contains("incorrect", result,
                StringComparison.OrdinalIgnoreCase); // Ensure error message mentions incorrect current password
        }
    }
}