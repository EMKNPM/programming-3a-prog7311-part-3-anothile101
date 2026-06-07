using PracticeAssignment.Models;

namespace PracticeAssignment.API.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllClientsAsync();
    Task<Client?> GetClientByIdAsync(int id);
    Task<Client> CreateClientAsync(Client client);
    Task<Client> UpdateClientAsync(Client client);
    Task<bool> DeleteClientAsync(int id);
    Task<bool> CanDeleteClientAsync(int id);
}