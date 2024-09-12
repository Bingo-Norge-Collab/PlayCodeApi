using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlayCodeApi.Contract.V1;
using PlayCodeApi.PlayCodes;

namespace PlayCodeApi.Proxy.ApiHandler;

public interface IHttpPlayCodeApiHandler : IProxyPlayCodeApiHandler { }

public class HttpPlayCodeApiHandler : IHttpPlayCodeApiHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private Uri _baseUrl;
    
    private JsonSerializerOptions? _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public HttpPlayCodeApiHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    protected virtual Task<HttpClient> NewClient()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        client.BaseAddress = _baseUrl;
        //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        return Task.FromResult(client);
    }

    public Task ConfigureAsync(string baseUrl)
    {
        _baseUrl = new Uri(baseUrl);
        return Task.CompletedTask;
    }

    public async Task<PlayCodeData?> GetPlayCodeAsync(int systemId, int locationId, string code)
    {
        var client = await NewClient();
        var url = ApiRoutes.PlayCodes.Get
            .Replace("{systemId}", systemId.ToString())
            .Replace("{locationId}", locationId.ToString())
            .Replace("{code}", code);

        var response = await client.GetAsync(url);
        return await BuildResponse<PlayCodeData>(response);
    }

    public async Task<PlayCodeData> CreatePlayCodeAsync(int systemId, int locationId, decimal amount)
    {
        var client = await NewClient();
        var url = ApiRoutes.PlayCodes.Purchase
            .Replace("{systemId}", systemId.ToString())
            .Replace("{locationId}", locationId.ToString());

        var body = new PlayCodePurchase(amount);
        var response = await client.PostAsJsonAsync(url, body, _jsonSerializerOptions);
        return await BuildResponse<PlayCodeData>(response);
    }

    public async Task<CashoutResult> CashOutPlayCodeAsync(int systemId, int locationId, string code)
    {
        var client = await NewClient();
        var url = ApiRoutes.PlayCodes.CashOut
            .Replace("{systemId}", systemId.ToString())
            .Replace("{locationId}", locationId.ToString())
            .Replace("{code}", code);

        var response = await client.PostAsync(url, null);
        return await BuildResponse<CashoutResult>(response);
    }

    public async Task<PlayCodeData> TopUpPlayCodeAsync(int systemId, int locationId, string code, decimal amount)
    {
        var client = await NewClient();
        var url = ApiRoutes.PlayCodes.TopUp
            .Replace("{systemId}", systemId.ToString())
            .Replace("{locationId}", locationId.ToString())
            .Replace("{code}", code);

        var body = new PlayCodeTopUp(amount);
        var response = await client.PostAsJsonAsync(url, body, _jsonSerializerOptions);
        return await BuildResponse<PlayCodeData>(response);
    }
    
    #region Helpers

    private async Task<T> BuildResponse<T>(HttpResponseMessage response)
        where T : class
    {
        if (response.Content == null)
            throw new PlayCodeException("Response was null", response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();

            var data = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
            if (data == null)
                throw new PlayCodeException("Failed to deserialize response", response.StatusCode);

            return data;
        }
        else
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ProblemDetails>(json, _jsonSerializerOptions);
            if (data != null)
                throw new PlayCodeException(data.Detail, response.StatusCode);
        }

        throw new PlayCodeException("Unknown error", response.StatusCode);
    }
    
    #endregion
}