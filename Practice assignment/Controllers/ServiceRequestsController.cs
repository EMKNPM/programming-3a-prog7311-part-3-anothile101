using Microsoft.AspNetCore.Mvc;
using Practice_assignment.Patterns.Repository;
using Practice_assignment.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Practice_assignment.Models;
using Practice_assignment.ViewModels;


  
    namespace Practice_assignment.Controllers;

    public class ServiceRequestsController : Controller
    {
        private readonly IApiService _apiService;

        public ServiceRequestsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index(int? contractId)
        {
            ViewBag.ContractId = contractId;

            if (contractId.HasValue)
            {
                var contract = await _apiService.GetContractAsync(contractId.Value);
                ViewBag.Contract = contract;
                ViewBag.Title = $"Service Requests for Contract #{contractId}";
            }
            else
            {
                ViewBag.Title = "All Service Requests";
            }

            var requests = await _apiService.GetServiceRequestsAsync(contractId);
            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var request = await _apiService.GetServiceRequestAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var contract = await _apiService.GetContractAsync(request.ContractId);
            ViewBag.Contract = contract;

            return View(request);
        }

        // GET: ServiceRequests/Create
        [HttpGet]
        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _apiService.GetContractAsync(contractId);
            if (contract == null)
            {
                return NotFound();
            }

            // Check if contract allows service requests
            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                TempData["Error"] = $"Cannot create service requests for contract with status '{contract.Status}'. Only Active or Draft contracts are allowed.";
                return RedirectToAction("Details", "Contracts", new { id = contractId });
            }

            ViewBag.Contract = contract;
            ViewBag.ExchangeRate = await _apiService.GetExchangeRateAsync();

            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int contractId, string description, decimal costUsd)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                ModelState.AddModelError("description", "Description is required.");
            }

            if (costUsd <= 0)
            {
                ModelState.AddModelError("costUsd", "Cost must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var request = await _apiService.CreateServiceRequestAsync(contractId, description, costUsd);

                    if (request != null)
                    {
                        TempData["Success"] = $"Service request created successfully! Cost: ${costUsd:N2} USD / {request.CostZar:C} ZAR";
                        return RedirectToAction("Details", "Contracts", new { id = contractId });
                    }
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            // If we got here, something failed
            ViewBag.Contract = await _apiService.GetContractAsync(contractId);
            ViewBag.ExchangeRate = await _apiService.GetExchangeRateAsync();
            return View();
        }

        // POST: ServiceRequests/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, ServiceRequestStatus status)
        {
            var request = await _apiService.GetServiceRequestAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var result = await _apiService.UpdateServiceRequestStatusAsync(id, status);

            if (result)
            {
                TempData["Success"] = $"Service request #{id} status updated to {status}!";
            }
            else
            {
                TempData["Error"] = "Failed to update status.";
            }

            return RedirectToAction("Details", "Contracts", new { id = request.ContractId });
        }

        // GET: ServiceRequests/Edit/5 (Quick edit status)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = await _apiService.GetServiceRequestAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            ViewBag.Statuses = Enum.GetValues(typeof(ServiceRequestStatus));
            return View(request);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequestStatus status)
        {
            return await UpdateStatus(id, status);
        }
    }