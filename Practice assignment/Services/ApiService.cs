using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Practice_assignment.Models;

namespace Practice_assignment.Services;


    public class ApiService : IApiService
    {
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TokenKey = "JwtToken";
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
    }

    // Private Helper Methods
   

    private void SetAuthHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenKey);
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

   
    // Authentication Methods
    
    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new { username, password });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
            if (result?.token != null)
            {
                _httpContextAccessor.HttpContext?.Session.SetString(TokenKey, result.token);
                SetAuthHeader();
                return true;
            }
        }
        return false;
    }

    public async Task LogoutAsync()
    {
        _httpContextAccessor.HttpContext?.Session.Remove(TokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

   
    // Client Methods
   

    public async Task<List<Client>> GetClientsAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("api/Clients");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Client>>(_jsonOptions) ?? new List<Client>();
    }

    public async Task<Client?> GetClientAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"api/Clients/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Client>(_jsonOptions);
    }

    public async Task<Client?> CreateClientAsync(Client client)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Clients", client, _jsonOptions);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Client>(_jsonOptions);

        var error = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Failed to create client: {error}");
    }

    public async Task<bool> UpdateClientAsync(Client client)
    {
        SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/Clients/{client.Id}", client, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.DeleteAsync($"api/Clients/{id}");
        return response.IsSuccessStatusCode;
    }

   
    // Contract Methods
    

    public async Task<List<Contract>> GetContractsAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("api/Contracts");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Contract>>(_jsonOptions) ?? new List<Contract>();
    }

    public async Task<Contract?> GetContractAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"api/Contracts/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<Contract>(_jsonOptions);
    }

    public async Task<Contract?> CreateContractAsync(int clientId, DateTime startDate, DateTime endDate, string serviceLevel)
    {
        SetAuthHeader();
        var request = new { clientId, startDate, endDate, serviceLevel };
        var response = await _httpClient.PostAsJsonAsync("api/Contracts", request, _jsonOptions);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Contract>(_jsonOptions);

        var error = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(error);
    }

    public async Task<bool> UpdateContractStatusAsync(int id, ContractStatus status)
    {
        SetAuthHeader();
        var response = await _httpClient.PatchAsJsonAsync($"api/Contracts/{id}/status", new { status }, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Contract>> FilterContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
    {
        SetAuthHeader();
        var query = new List<string>();
        if (startDate.HasValue) query.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) query.Add($"endDate={endDate.Value:yyyy-MM-dd}");
        if (status.HasValue) query.Add($"status={(int)status.Value}");

        var url = "api/Contracts/filter";
        if (query.Any()) url += "?" + string.Join("&", query);

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Contract>>(_jsonOptions) ?? new List<Contract>();
    }

    
    // Service Request Methods
    

    public async Task<List<ServiceRequest>> GetServiceRequestsAsync(int? contractId = null)
    {
        SetAuthHeader();
        var url = contractId.HasValue ? $"api/ServiceRequests/contract/{contractId.Value}" : "api/ServiceRequests";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<ServiceRequest>>(_jsonOptions) ?? new List<ServiceRequest>();
    }

    public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync($"api/ServiceRequests/{id}");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ServiceRequest>(_jsonOptions);
    }

    public async Task<ServiceRequest?> CreateServiceRequestAsync(int contractId, string description, decimal costUsd)
    {
        SetAuthHeader();
        var request = new { contractId, description, costUsd };
        var response = await _httpClient.PostAsJsonAsync("api/ServiceRequests", request, _jsonOptions);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<ServiceRequest>(_jsonOptions);

        var error = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException(error);
    }

    public async Task<bool> UpdateServiceRequestStatusAsync(int id, ServiceRequestStatus status)
    {
        SetAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"api/ServiceRequests/{id}/status", new { status }, _jsonOptions);
        return response.IsSuccessStatusCode;
    }

   
    // Currency Methods
    
    public async Task<decimal> GetExchangeRateAsync()
    {
        SetAuthHeader();
        var response = await _httpClient.GetAsync("api/Currency/rate");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ExchangeRateResponse>(_jsonOptions);
        return result?.Rate ?? 18.50m;
    }

    public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
    {
        SetAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("api/Currency/convert", new { usdAmount }, _jsonOptions);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ConvertResponse>(_jsonOptions);
        return result?.ZarAmount ?? usdAmount * 18.50m;
    }

    
    // Private Response Classes
    

    private class LoginResponse
    {
        public string token { get; set; } = string.Empty;
    }

    private class ExchangeRateResponse
    {
        public decimal Rate { get; set; }
    }

    private class ConvertResponse
    {
        public decimal ZarAmount { get; set; }
    }
}
