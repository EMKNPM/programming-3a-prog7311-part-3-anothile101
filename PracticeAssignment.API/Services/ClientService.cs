using PracticeAssignment.Models;
using PracticeAssignment.API.Repositories;

namespace PracticeAssignment.API.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;

    public ClientService(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<Client>> GetAllClientsAsync()
    {
        return await _clientRepository.GetAllAsync();
    }

    public async Task<Client?> GetClientByIdAsync(int id)
    {
        return await _clientRepository.GetByIdAsync(id);
    }

    public async Task<Client> CreateClientAsync(Client client)
    {
        return await _clientRepository.AddAsync(client);
    }

    public async Task<Client> UpdateClientAsync(Client client)
    {
        var existing = await _clientRepository.GetByIdAsync(client.Id);
        if (existing == null)
            throw new ArgumentException($"Client with ID {client.Id} not found.");

        return await _clientRepository.UpdateAsync(client);
    }

    public async Task<bool> DeleteClientAsync(int id)
    {
        var canDelete = await CanDeleteClientAsync(id);
        if (!canDelete)
            throw new InvalidOperationException("Cannot delete client with existing contracts.");

        return await _clientRepository.DeleteAsync(id);
    }

    public async Task<bool> CanDeleteClientAsync(int id)
    {
        return !await _clientRepository.HasContractsAsync(id);
    }
}