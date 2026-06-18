using MyFinance.Models;

namespace MyFinance.Services;

public interface IFinanceApiClient
{
    Task<FinanceApiResponse<IReadOnlyList<AccountViewModel>>> GetAccountsAsync();
    Task<FinanceApiResponse<AccountViewModel>> GetAccountAsync(int id);
    Task<FinanceApiResponse> CreateAccountAsync(AccountViewModel account);
    Task<FinanceApiResponse> UpdateAccountAsync(int id, AccountViewModel account);
    Task<FinanceApiResponse> DeleteAccountAsync(int id);
    Task<FinanceApiResponse<string>> AuthenticateAsync(LoginViewModel login);
}
