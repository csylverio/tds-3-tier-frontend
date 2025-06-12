using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using System.Security.Claims;
using System.Text.Json;

namespace MyFinance.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IHttpClientFactory httpFactory, ILogger<AccountController> logger)
    {
        _httpFactory = httpFactory;
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
            var client = _httpFactory.CreateClient("FinanceApi");
            var resp = await client.PostAsJsonAsync("api/Token", vm);

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Usuário ou senha inválidos.");
                return View(vm);
            }

            var json = await resp.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<string>(json) ?? "";
            if (string.IsNullOrWhiteSpace(token))
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
                Value = token
            }});

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);
            return vm.ReturnUrl != null ? Redirect(vm.ReturnUrl) : RedirectToAction("Index", "Accounts");
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
