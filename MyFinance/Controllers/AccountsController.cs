using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace MyFinance.Controllers;

[Authorize]
public class AccountsController : Controller
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IHttpClientFactory httpFactory, ILogger<AccountsController> logger)
    {
        _httpFactory = httpFactory;
        _logger = logger;
    }

    private async Task<HttpClient> CreateClientWithToken()
    {
        var client = _httpFactory.CreateClient("FinanceApi");
        var token = await HttpContext.GetTokenAsync("access_token");

        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    // GET: Accounts
    public async Task<IActionResult> Index()
    {
        try
        {
            var client = await CreateClientWithToken();
            var response = await client.GetAsync("/api/Account");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
            }

            var accounts = await response.Content.ReadFromJsonAsync<List<AccountViewModel>>();

            // Adiciona DataExecucao para cada conta
            var accountsWithDate = accounts?.Select(a =>
            {
                a.DataExecucao = DateTime.Now;
                return a;
            }).ToList()
            .OrderBy(x => x.Name); // ordena lista de retorno

            return View(accountsWithDate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter contas");
            return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
        }
    }

    // GET: Accounts/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        try
        {
            var client = await CreateClientWithToken();
            var response = await client.GetAsync($"/api/Account/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter detalhes da conta. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter detalhes da conta." });
            }

            var account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
            return View(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter detalhes da conta.");
            return RedirectToAction("Error", "Home", new { message = "Erro ao obter detalhes da conta." });
        }
    }

    // GET: Accounts/Create
    public IActionResult Create() => View();

    // POST: Accounts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Balance")] AccountViewModel account)
    {
        if (!ModelState.IsValid)
            return View(account);

        try
        {
            var client = await CreateClientWithToken();
            var response = await client.PostAsJsonAsync("/api/Account", account);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao criar conta. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao criar conta." });
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar conta");
            return RedirectToAction("Error", "Home", new { message = "Erro ao criar conta." });
        }
    }

    // GET: Accounts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        try
        {
            var client = await CreateClientWithToken();
            var response = await client.GetAsync($"/api/Account/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
            }

            var account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
            return View(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter contas");
            return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
        }
    }

    // POST: Accounts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Balance")] AccountViewModel account)
    {
        if (id != account.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(account);

        try
        {
            var client = await CreateClientWithToken();
            var response = await client.PutAsJsonAsync($"/api/Account/{id}", account);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao atualizar conta. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao atualizar contas." });
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar contas.");
            return RedirectToAction("Error", "Home", new { message = "Erro ao atualizar contas." });
        }
    }

    // GET: Accounts/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        try
        {
            var client = await CreateClientWithToken();
            var response = await client.GetAsync($"/api/Account/{id}"); ;

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
            }

            var account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
            return View(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter contas");
            return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
        }
    }

    // POST: Accounts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var client = await CreateClientWithToken();
            var response = await client.DeleteAsync($"/api/Account/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (response.StatusCode == HttpStatusCode.Forbidden)
                return RedirectToAction("Error", "Home", new { message = "Permissão negada para operação." });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao deletar conta. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao deletar conta." });
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar conta.");
            return RedirectToAction("Error", "Home", new { message = "Erro ao deletar conta." });
        }
    }
}
