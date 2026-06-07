
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PracticeAssignment.Models;
using Xunit;

namespace Practice_assignment.Tests.IntegrationTests;

public class ClientsApiIntegrationTests
{
    private readonly HttpClient _client;
    private string _authToken = string.Empty;

    public ClientsApiIntegrationTests()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://localhost:7217");
    }

    private async Task AuthenticateAsync()
    {
        if (string.IsNullOrEmpty(_authToken))
        {
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", new
            {
                username = "admin",
                password = "TechMove2026!"
            });

            var result = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
            _authToken = result?.token ?? string.Empty;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }
    }

    [Fact]
    public async Task GetAllClients_WithValidToken_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/Clients");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var clients = await response.Content.ReadFromJsonAsync<List<Client>>();
        Assert.NotNull(clients);
    }

    [Fact]
    public async Task GetAllClients_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/Clients");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetClientById_WithValidId_ReturnsClient()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/Clients/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var client = await response.Content.ReadFromJsonAsync<Client>();
        Assert.NotNull(client);
        Assert.Equal(1, client.Id);
    }

    private class LoginResponse
    {
        public string token { get; set; } = string.Empty;
    }
}