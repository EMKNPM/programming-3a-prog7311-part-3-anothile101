using Microsoft.AspNetCore.Mvc;
using Practice_assignment.Models;
using Practice_assignment.Patterns.Repository;
using Practice_assignment.Services;

namespace Practice_assignment.Controllers;


    public class ClientsController : Controller
    {
        private readonly IApiService _apiService;

        public ClientsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _apiService.GetClientsAsync();
            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var client = await _apiService.GetClientAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            // Get contracts for this client
            var allContracts = await _apiService.GetContractsAsync();
            var clientContracts = allContracts.Where(c => c.ClientId == id).ToList();
            ViewBag.Contracts = clientContracts;

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                var created = await _apiService.CreateClientAsync(client);
                if (created != null)
                {
                    TempData["Success"] = $"Client '{created.Name}' created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create client. Please try again.");
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _apiService.GetClientAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var updated = await _apiService.UpdateClientAsync(client);
                if (updated)
                {
                    TempData["Success"] = $"Client '{client.Name}' updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update client.");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var client = await _apiService.GetClientAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _apiService.DeleteClientAsync(id);
            if (deleted)
            {
                TempData["Success"] = "Client deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete client. It may have existing contracts.";
            }
            return RedirectToAction(nameof(Index));
        }
    }