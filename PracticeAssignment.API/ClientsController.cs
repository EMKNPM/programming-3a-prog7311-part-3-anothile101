using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeAssignment.Data;
using PracticeAssignment.Models;

namespace PracticeAssignment.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _context.Clients.ToListAsync();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound(new { message = $"Client {id} not found" });

        return Ok(client);
    }

    // ADD THIS POST METHOD ↓↓↓
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Client client)
    {
        if (client == null)
            return BadRequest(new { message = "Client data is required" });

        if (string.IsNullOrWhiteSpace(client.Name))
            return BadRequest(new { message = "Client name is required" });

        if (string.IsNullOrWhiteSpace(client.ContactDetails))
            return BadRequest(new { message = "Contact details are required" });

        if (string.IsNullOrWhiteSpace(client.Region))
            return BadRequest(new { message = "Region is required" });

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Client client)
    {
        if (id != client.Id)
            return BadRequest(new { message = "ID mismatch" });

        var existing = await _context.Clients.FindAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Client {id} not found" });

        existing.Name = client.Name;
        existing.ContactDetails = client.ContactDetails;
        existing.Region = client.Region;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null)
            return NotFound(new { message = $"Client {id} not found" });

        var hasContracts = await _context.Contracts.AnyAsync(c => c.ClientId == id);
        if (hasContracts)
            return BadRequest(new { message = "Cannot delete client with existing contracts" });

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}