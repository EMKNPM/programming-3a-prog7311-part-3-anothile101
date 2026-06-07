using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Practice_assignment.Models;
using Practice_assignment.Patterns.Repository;
using Practice_assignment.Services;
using Practice_assignment.ViewModels;

namespace Practice_assignment.Controllers;
    

    public class ContractsController : Controller
    {
        private readonly IApiService _apiService;

        public ContractsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _apiService.FilterContractsAsync(startDate, endDate, status);
            ViewBag.Clients = await _apiService.GetClientsAsync();
            ViewBag.CurrentStartDate = startDate;
            ViewBag.CurrentEndDate = endDate;
            ViewBag.CurrentStatus = status;
            return View(contracts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _apiService.GetContractAsync(id);
            if (contract == null) return NotFound();

            var serviceRequests = await _apiService.GetServiceRequestsAsync(id);
            ViewBag.ServiceRequests = serviceRequests;
            return View(contract);
        }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var clients = await _apiService.GetClientsAsync();
        ViewBag.Clients = clients.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name
        }).ToList();

        return View();
    }
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clientId, DateTime startDate, DateTime endDate, string serviceLevel)
        {
            try
            {
                var contract = await _apiService.CreateContractAsync(clientId, startDate, endDate, serviceLevel);
                TempData["Success"] = $"Contract {contract?.Id} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Clients = await _apiService.GetClientsAsync();
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, ContractStatus status)
        {
            var result = await _apiService.UpdateContractStatusAsync(id, status);
            TempData["Success"] = result ? $"Status updated to {status}" : "Update failed";
            return RedirectToAction(nameof(Details), new { id });
        }
    }