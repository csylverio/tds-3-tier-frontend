using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using MyFinance.Models;

namespace MyFinance.Services;

public sealed class FinanceApiClient : IFinanceApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FinanceApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FinanceApiResponse<IReadOnlyList<AccountViewModel>>> GetAccountsAsync()
    {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "/api/Account");
        var response = await SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new FinanceApiResponse<IReadOnlyList<AccountViewModel>>(response.StatusCode, null);

        var accounts = await response.Content.ReadFromJsonAsync<List<AccountViewModel>>();
        return new FinanceApiResponse<IReadOnlyList<AccountViewModel>>(
            response.StatusCode,
            accounts ?? new List<AccountViewModel>());
    }

    public async Task<FinanceApiResponse<AccountViewModel>> GetAccountAsync(int id)
    {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, $"/api/Account/{id}");
        var response = await SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return new FinanceApiResponse<AccountViewModel>(response.StatusCode, null);

        var account = await response.Content.ReadFromJsonAsync<AccountViewModel>();
        return new FinanceApiResponse<AccountViewModel>(response.StatusCode, account);
    }

    public async Task<FinanceApiResponse> CreateAccountAsync(AccountViewModel account)
    {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, "/api/Account");
        request.Content = JsonContent.Create(account);

        var response = await SendAsync(request);
        return new FinanceApiResponse(response.StatusCode);
    }

    public async Task<FinanceApiResponse> UpdateAccountAsync(int id, AccountViewModel account)
    {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Put, $"/api/Account/{id}");
        request.Content = JsonContent.Create(account);

        var response = await SendAsync(request);
        return new FinanceApiResponse(response.StatusCode);
    }

    public async Task<FinanceApiResponse> DeleteAccountAsync(int id)
    {
        var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"/api/Account/{id}");
        var response = await SendAsync(request);

        return new FinanceApiResponse(response.StatusCode);
    }

    public async Task<FinanceApiResponse<string>> AuthenticateAsync(LoginViewModel login)
    {
        var response = await Client.PostAsJsonAsync("api/Token", login);

        if (!response.IsSuccessStatusCode)
            return new FinanceApiResponse<string>(response.StatusCode, null);

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<string>(json);

        return new FinanceApiResponse<string>(response.StatusCode, token);
    }

    private HttpClient Client => _httpClientFactory.CreateClient("FinanceApi");

    private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string requestUri)
    {
        var request = new HttpRequestMessage(method, requestUri);
        var httpContext = _httpContextAccessor.HttpContext;
        var token = httpContext is null ? null : await httpContext.GetTokenAsync("access_token");

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return request;
    }

    private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
    {
        return Client.SendAsync(request);
    }
}
