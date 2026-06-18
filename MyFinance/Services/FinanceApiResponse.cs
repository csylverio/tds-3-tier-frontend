using System.Net;

namespace MyFinance.Services;

public sealed record FinanceApiResponse<T>(HttpStatusCode StatusCode, T? Value)
{
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;
}

public sealed record FinanceApiResponse(HttpStatusCode StatusCode)
{
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;
}
