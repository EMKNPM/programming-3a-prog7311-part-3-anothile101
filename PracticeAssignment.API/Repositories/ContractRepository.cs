using Microsoft.EntityFrameworkCore;
using PracticeAssignment.Data;
using PracticeAssignment.Models;

namespace PracticeAssignment.API.Repositories;

public class ContractRepository : IContractRepository
{
    private readonly ApplicationDbContext _context;

    public ContractRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        return await _context.Contracts
            .Include(c => c.Client)
            .ToListAsync();
    }

    public async Task<Contract?> GetByIdAsync(int id)
    {
        return await _context.Contracts
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contract>> GetByClientIdAsync(int clientId)
    {
        return await _context.Contracts
            .Include(c => c.Client)
            .Where(c => c.ClientId == clientId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetByDateRangeAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Contracts.Include(c => c.Client).AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.StartDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(c => c.EndDate <= endDate.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status)
    {
        return await _context.Contracts
            .Include(c => c.Client)
            .Where(c => c.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Contract>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
    {
        var query = _context.Contracts.Include(c => c.Client).AsQueryable();

        if (startDate.HasValue)
            query = query.Where(c => c.StartDate >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(c => c.EndDate <= endDate.Value);
        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        return await query.ToListAsync();
    }

    public async Task<Contract> AddAsync(Contract contract)
    {
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task<Contract> UpdateAsync(Contract contract)
    {
        _context.Entry(contract).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return contract;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            return false;

        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Contracts.AnyAsync(c => c.Id == id);
    }
}