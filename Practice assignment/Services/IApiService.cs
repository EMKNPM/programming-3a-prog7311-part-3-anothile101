
using Practice_assignment.Models;

namespace Practice_assignment.Services;

    public interface IApiService
    {

    // Authentication
    
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();

    
    // Clients
   
    Task<List<Client>> GetClientsAsync();
    Task<Client?> GetClientAsync(int id);
    Task<Client?> CreateClientAsync(Client client);
    Task<bool> UpdateClientAsync(Client client);
    Task<bool> DeleteClientAsync(int id);

    
    // Contracts
   
    Task<List<Contract>> GetContractsAsync();
    Task<Contract?> GetContractAsync(int id);
    Task<Contract?> CreateContractAsync(int clientId, DateTime startDate, DateTime endDate, string serviceLevel);
    Task<bool> UpdateContractStatusAsync(int id, ContractStatus status);
    Task<List<Contract>> FilterContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);

  
    // Service Requests
    
    Task<List<ServiceRequest>> GetServiceRequestsAsync(int? contractId = null);
    Task<ServiceRequest?> GetServiceRequestAsync(int id);
    Task<ServiceRequest?> CreateServiceRequestAsync(int contractId, string description, decimal costUsd);
    Task<bool> UpdateServiceRequestStatusAsync(int id, ServiceRequestStatus status);

   
    // Currency
    
    Task<decimal> GetExchangeRateAsync();
    Task<decimal> ConvertUsdToZarAsync(decimal usdAmount);
}
