using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeAssignment.Data;
using PracticeAssignment.Models;

namespace PracticeAssignment.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ServiceRequestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public ServiceRequestsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var requests = await _context.ServiceRequests
            .Include(r => r.Contract)
            .ToListAsync();
        return Ok(requests);
    }

    [HttpGet("contract/{contractId}")]
    public async Task<IActionResult> GetByContractId(int contractId)
    {
        var requests = await _context.ServiceRequests
            .Where(r => r.ContractId == contractId)
            .ToListAsync();
        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var request = await _context.ServiceRequests
            .Include(r => r.Contract)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
            return NotFound(new { message = $"Service request {id} not found" });

        return Ok(request);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request)
    {
        var contract = await _context.Contracts.FindAsync(request.ContractId);
        if (contract == null)
            return BadRequest(new { message = "Contract not found" });

        if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            return BadRequest(new { message = $"Cannot create service request for contract with status '{contract.Status}'. Only Active or Draft contracts are allowed." });

        var rate = await GetExchangeRateAsync();
        var zarCost = Math.Round(request.CostUsd * rate, 2);

        var serviceRequest = new ServiceRequest
        {
            ContractId = request.ContractId,
            Description = request.Description,
            CostUsd = request.CostUsd,
            CostZar = zarCost,
            ExchangeRateUsed = rate,
            Status = ServiceRequestStatus.Pending
        };

        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = serviceRequest.Id }, serviceRequest);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateServiceRequestStatus request)
    {
        var serviceRequest = await _context.ServiceRequests.FindAsync(id);
        if (serviceRequest == null)
            return NotFound(new { message = $"Service request {id} not found" });

        serviceRequest.Status = request.Status;
        await _context.SaveChangesAsync();

        return Ok(new { message = $"Status updated to {request.Status}" });
    }
    private async Task<decimal> GetExchangeRateAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ExchangeRateResponse>();
                if (result?.rates != null && result.rates.TryGetValue("ZAR", out var rate))
                    return rate;
            }
        }
        catch { }
        return 18.50m;
    }
}

public class CreateServiceRequest
{
    public int ContractId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal CostUsd { get; set; }
}

public class UpdateServiceRequestStatus
{
    public ServiceRequestStatus Status { get; set; }
}

public class ExchangeRateResponse
{
    public Dictionary<string, decimal>? rates { get; set; }
}