using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PracticeAssignment.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CurrencyController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public CurrencyController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet("rate")]
    public async Task<IActionResult> GetRate()
    {
        var rate = await GetExchangeRateAsync();
        return Ok(new { Rate = rate, Currency = "USD/ZAR", Timestamp = DateTime.UtcNow });
    }

    [HttpPost("convert")]
    public async Task<IActionResult> Convert([FromBody] ConvertRequest request)
    {
        var rate = await GetExchangeRateAsync();
        var zarAmount = Math.Round(request.UsdAmount * rate, 2);

        return Ok(new
        {
            UsdAmount = request.UsdAmount,
            ZarAmount = zarAmount,
            ExchangeRate = rate,
            ConvertedAt = DateTime.UtcNow
        });
    }

    private async Task<decimal> GetExchangeRateAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ExchangeRateApiResponse>();
                if (result?.rates != null && result.rates.TryGetValue("ZAR", out var rate))
                    return rate;
            }
        }
        catch { }
        return 18.50m;
    }
}

public class ConvertRequest
{
    public decimal UsdAmount { get; set; }
}

public class ExchangeRateApiResponse
{
    public Dictionary<string, decimal>? rates { get; set; }
}