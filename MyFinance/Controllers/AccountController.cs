using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using MyFinance.Services;
using System.Security.Claims;

namespace MyFinance.Controllers;

public class AccountController : Controller
{
    private readonly IFinanceApiClient _financeApiClient;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IFinanceApiClient financeApiClient, ILogger<AccountController> logger)
    {
        _financeApiClient = financeApiClient;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var response = await _financeApiClient.AuthenticateAsync(vm);

            if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(response.Value))
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View(vm);
            }
            
            var claims = new[] { new Claim(ClaimTypes.Name, vm.Username ?? string.Empty) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProps = new AuthenticationProperties { IsPersistent = false };

            authProps.StoreTokens(new[] { new AuthenticationToken
            {
                Name = "access_token",
                Value = response.Value
            }});

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return LocalRedirect(vm.ReturnUrl);

            return RedirectToAction("Index", "Accounts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login");
            ModelState.AddModelError("", "Erro de comunicação. Tente novamente.");
            return View(vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
