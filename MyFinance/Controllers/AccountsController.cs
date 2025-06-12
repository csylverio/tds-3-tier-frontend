using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;

namespace MyFinance.Controllers;

public class AccountsController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IHttpClientFactory httpClientFactory, 
                           ILogger<AccountsController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("FinanceApi");
        _logger = logger;
    }

    // GET: Accounts
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync("accounts");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return View("Error");
            }

            var accounts = await response.Content.ReadFromJsonAsync<List<AccountViewModel>>();
            
            // Adiciona DataExecucao para cada conta
            var accountsWithDate = accounts?.Select(a => 
            {
                a.DataExecucao = DateTime.Now;
                return a;
            }).ToList();

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
        {
            return NotFound();
        }

        try
        {
            var response = await _httpClient.GetAsync($"accounts/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter detalhes da conta. Status: {StatusCode}", response.StatusCode);
               return RedirectToAction("Error", "Home", new { message = "Erro ao obter detalhes da conta."});
            }

            var account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
            return View(account);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter detalhes da conta.");
             return RedirectToAction("Error", "Home", new { message = "Erro ao obter detalhes da conta."});
        }
    }

    // GET: Accounts/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Accounts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Balance")] AccountViewModel account)
    {
        if (!ModelState.IsValid)
        {
            return View(account);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("accounts", account);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao criar conta. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao criar conta."});
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar conta");
            return RedirectToAction("Error", "Home", new { message = "Erro ao criar conta."});
        }
    }

    // GET: Accounts/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var response = await _httpClient.GetAsync($"accounts/{id}");
            
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
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(account);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"accounts/{id}", account);
            
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
        {
            return NotFound();
        }

        try
        {
            var response = await _httpClient.GetAsync($"accounts/{id}");
            
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
            var response = await _httpClient.DeleteAsync($"accounts/{id}");
            
            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

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
