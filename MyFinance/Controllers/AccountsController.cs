using System.Net;
using Microsoft.AspNetCore.Mvc;
using MyFinance.Models;
using Microsoft.AspNetCore.Authorization;
using MyFinance.Services;

namespace MyFinance.Controllers;

[Authorize]
public class AccountsController : Controller
{
    private readonly IFinanceApiClient _financeApiClient;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IFinanceApiClient financeApiClient, ILogger<AccountsController> logger)
    {
        _financeApiClient = financeApiClient;
        _logger = logger;
    }

    // GET: Accounts
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _financeApiClient.GetAccountsAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
            }

            var accounts = response.Value ?? Array.Empty<AccountViewModel>();
            var accountsWithDate = accounts.Select(a =>
            {
                a.DataExecucao = DateTimeOffset.UtcNow;
                return a;
            }).ToList()
            .OrderBy(x => x.Name);

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
            var response = await _financeApiClient.GetAccountAsync(id.Value);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter detalhes da conta. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter detalhes da conta." });
            }

            if (response.Value is null)
                return RedirectToAction("Error", "Home", new { message = "Conta não encontrada." });

            return View(response.Value);
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
            var response = await _financeApiClient.CreateAccountAsync(account);

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
            var response = await _financeApiClient.GetAccountAsync(id.Value);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
            }

            if (response.Value is null)
                return RedirectToAction("Error", "Home", new { message = "Conta não encontrada." });

            return View(response.Value);
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
            var response = await _financeApiClient.UpdateAccountAsync(id, account);

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
            var response = await _financeApiClient.GetAccountAsync(id.Value);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro ao obter contas. Status: {StatusCode}", response.StatusCode);
                return RedirectToAction("Error", "Home", new { message = "Erro ao obter contas." });
            }

            if (response.Value is null)
                return RedirectToAction("Error", "Home", new { message = "Conta não encontrada." });

            return View(response.Value);
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
            var response = await _financeApiClient.DeleteAccountAsync(id);

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
