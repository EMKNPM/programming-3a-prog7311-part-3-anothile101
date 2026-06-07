using Microsoft.AspNetCore.Mvc;
using Practice_assignment.Services;

namespace Practice_assignment.Controllers;

public class AccountController : Controller
{
    private readonly IApiService _apiService;

    public AccountController(IApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password)
    {
        var success = await _apiService.LoginAsync(username, password);
        if (success)
        {
            return RedirectToAction("Index", "Contracts");
        }
        ViewBag.Error = "Invalid username or password";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await _apiService.LogoutAsync();
        return RedirectToAction("Login");
    }
}