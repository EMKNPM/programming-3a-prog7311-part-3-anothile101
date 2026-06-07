using PracticeAssignment.Models;

namespace PracticeAssignment.API.Services;

public interface IContractService
{
    Task<IEnumerable<Contract>> GetAllContractsAsync();
    Task<Contract?> GetContractByIdAsync(int id);
    Task<IEnumerable<Contract>> GetContractsByClientIdAsync(int clientId);
    Task<IEnumerable<Contract>> GetFilteredContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
    Task<Contract> CreateContractAsync(int clientId, DateTime startDate, DateTime endDate, string serviceLevel);
    Task<Contract> UpdateContractStatusAsync(int id, ContractStatus status);
    Task<bool> DeleteContractAsync(int id);
    bool ValidateContractDates(DateTime startDate, DateTime endDate);
    ContractStatus GetDefaultStatusForServiceLevel(string serviceLevel);
}