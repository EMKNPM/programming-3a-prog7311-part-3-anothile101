using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeAssignment.Data;
using PracticeAssignment.Models;
using PracticeAssignment.API.Services;

namespace PracticeAssignment.API;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contracts = await _contractService.GetAllContractsAsync();
        return Ok(contracts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contract = await _contractService.GetContractByIdAsync(id);
        if (contract == null)
            return NotFound(new { message = $"Contract {id} not found" });

        return Ok(contract);
    }

    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] ContractStatus? status)
    {
        var contracts = await _contractService.GetFilteredContractsAsync(startDate, endDate, status);
        return Ok(contracts);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractRequest request)
    {
        try
        {
            var contract = await _contractService.CreateContractAsync(
                request.ClientId, request.StartDate, request.EndDate, request.ServiceLevel);

            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var contract = await _contractService.UpdateContractStatusAsync(id, request.Status);
            return Ok(new { message = $"Status updated to {request.Status}" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/servicerequests")]
    public async Task<IActionResult> GetServiceRequests(int id)
    {
        // This would call a service request service
        return Ok(new List<object>());
    }
}

public class CreateContractRequest
{
    public int ClientId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ServiceLevel { get; set; } = "Bronze";
}

public class UpdateStatusRequest
{
    public ContractStatus Status { get; set; }
}