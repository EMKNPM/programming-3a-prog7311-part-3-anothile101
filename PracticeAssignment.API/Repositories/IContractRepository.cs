using PracticeAssignment.Models;

namespace PracticeAssignment.API.Repositories;

public interface IContractRepository
{
    Task<IEnumerable<Contract>> GetAllAsync();
    Task<Contract?> GetByIdAsync(int id);
    Task<IEnumerable<Contract>> GetByClientIdAsync(int clientId);
    Task<IEnumerable<Contract>> GetByDateRangeAsync(DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status);
    Task<IEnumerable<Contract>> GetFilteredAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
    Task<Contract> AddAsync(Contract contract);
    Task<Contract> UpdateAsync(Contract contract);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}