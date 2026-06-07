
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PracticeAssignment.Models;
using Xunit;

namespace Practice_assignment.Tests.IntegrationTests;

public class ContractsApiIntegrationTests
{
    private readonly HttpClient _client;
    private string _authToken = string.Empty;

    public ContractsApiIntegrationTests()
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
    public async Task GetAllContracts_WithValidToken_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/Contracts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();
        Assert.NotNull(contracts);
    }

    [Fact]
    public async Task GetAllContracts_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/api/Contracts");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetContractById_WithValidId_ReturnsContract()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/Contracts/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var contract = await response.Content.ReadFromJsonAsync<Contract>();
        Assert.NotNull(contract);
        Assert.Equal(1, contract.Id);
    }

    [Fact]
    public async Task GetContractById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/Contracts/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateContract_WithValidData_ReturnsCreated()
    {
        // Arrange
        await AuthenticateAsync();

        var newContract = new
        {
            clientId = 1,
            startDate = DateTime.UtcNow.AddDays(1),
            endDate = DateTime.UtcNow.AddYears(1),
            serviceLevel = "Gold"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Contracts", newContract);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<Contract>();
        Assert.NotNull(created);
        Assert.Equal("Gold", created.ServiceLevel);
    }

    [Fact]
    public async Task CreateContract_WithInvalidDates_ReturnsBadRequest()
    {
        // Arrange
        await AuthenticateAsync();

        var invalidContract = new
        {
            clientId = 1,
            startDate = DateTime.UtcNow.AddYears(1),
            endDate = DateTime.UtcNow.AddDays(1),
            serviceLevel = "Bronze"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/Contracts", invalidContract);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateContractStatus_WithValidData_ReturnsOk()
    {
        // Arrange
        await AuthenticateAsync();

        var newContract = new
        {
            clientId = 1,
            startDate = DateTime.UtcNow.AddDays(1),
            endDate = DateTime.UtcNow.AddYears(1),
            serviceLevel = "Silver"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/Contracts", newContract);
        var created = await createResponse.Content.ReadFromJsonAsync<Contract>();

        // Act - Update status to Expired
        var updateResponse = await _client.PatchAsJsonAsync($"/api/Contracts/{created!.Id}/status", new
        {
            status = ContractStatus.Expired
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
    }

    [Fact]
    public async Task FilterContracts_ByStatus_ReturnsFilteredResults()
    {
        // Arrange
        await AuthenticateAsync();

        // Act
        var response = await _client.GetAsync("/api/Contracts/filter?status=0");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var contracts = await response.Content.ReadFromJsonAsync<List<Contract>>();
        Assert.NotNull(contracts);
    }

    private class LoginResponse
    {
        public string token { get; set; } = string.Empty;
    }
}