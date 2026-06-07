using PracticeAssignment.Models;
using PracticeAssignment.API.Repositories;

namespace PracticeAssignment.API.Services;

public class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly IClientRepository _clientRepository;

    public ContractService(IContractRepository contractRepository, IClientRepository clientRepository)
    {
        _contractRepository = contractRepository;
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<Contract>> GetAllContractsAsync()
    {
        return await _contractRepository.GetAllAsync();
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        return await _contractRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Contract>> GetContractsByClientIdAsync(int clientId)
    {
        return await _contractRepository.GetByClientIdAsync(clientId);
    }

    public async Task<IEnumerable<Contract>> GetFilteredContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
    {
        return await _contractRepository.GetFilteredAsync(startDate, endDate, status);
    }

    public async Task<Contract> CreateContractAsync(int clientId, DateTime startDate, DateTime endDate, string serviceLevel)
    {
        // Validate client exists
        var clientExists = await _clientRepository.ExistsAsync(clientId);
        if (!clientExists)
            throw new InvalidOperationException($"Client with ID {clientId} does not exist.");

        // Validate dates
        if (!ValidateContractDates(startDate, endDate))
            throw new InvalidOperationException("End date must be after start date.");

        // Get status based on service level
        var status = GetDefaultStatusForServiceLevel(serviceLevel);

        var contract = new Contract
        {
            ClientId = clientId,
            StartDate = startDate,
            EndDate = endDate,
            ServiceLevel = serviceLevel,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };

        return await _contractRepository.AddAsync(contract);
    }

    public async Task<Contract> UpdateContractStatusAsync(int id, ContractStatus status)
    {
        var contract = await _contractRepository.GetByIdAsync(id);
        if (contract == null)
            throw new ArgumentException($"Contract with ID {id} not found.");

        contract.Status = status;
        contract.UpdatedAt = DateTime.UtcNow;

        return await _contractRepository.UpdateAsync(contract);
    }

    public async Task<bool> DeleteContractAsync(int id)
    {
        return await _contractRepository.DeleteAsync(id);
    }

    public bool ValidateContractDates(DateTime startDate, DateTime endDate)
    {
        return endDate > startDate;
    }

    public ContractStatus GetDefaultStatusForServiceLevel(string serviceLevel)
    {
        return serviceLevel?.ToLower() == "gold" ? ContractStatus.Active : ContractStatus.Draft;
    }
}