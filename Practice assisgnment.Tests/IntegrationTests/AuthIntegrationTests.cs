

using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Practice_assignment.Tests.IntegrationTests;

public class AuthIntegrationTests
{
    private readonly HttpClient _client;

    public AuthIntegrationTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7217");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new { username = "admin", password = "TechMove2026!" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result?.token);
        Assert.NotEmpty(result?.token ?? "");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new { username = "wrong", password = "wrong" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new { username = "", password = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private class LoginResponse
    {
        public string token { get; set; } = string.Empty;
    }
}