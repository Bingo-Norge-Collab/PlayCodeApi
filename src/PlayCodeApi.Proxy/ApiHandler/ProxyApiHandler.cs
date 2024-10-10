using Microsoft.Extensions.Options;
using PlayCodeApi.Application;
using PlayCodeApi.Contract.V1;
using PlayCodeApi.Proxy.Config;
using PlayCodeApi.Proxy.Exceptions;
using PlayCodeApi.Proxy.OkBingo;

namespace PlayCodeApi.Proxy.ApiHandler;

/// <summary>
/// Forwards request to appropriate handler, depending on configuration (appsettings).
/// </summary>
public class ProxyApiHandler : IPlayCodeApiHandler
{
    private readonly IOptions<SystemOptions> _options;
    private readonly IOkBingoApiHandler _okBingoApiHandler;
    private readonly IHttpPlayCodeApiHandler _httpHandler;
    public ProxyApiHandler(IOptions<SystemOptions> options, IOkBingoApiHandler okBingoApiHandler, IHttpPlayCodeApiHandler httpHandler)
    {
        _options = options;
        _okBingoApiHandler = okBingoApiHandler;
        _httpHandler = httpHandler;
    }

    public async Task<PlayCodeData?> GetPlayCodeAsync(int systemId, int locationId, string code)
    {
        var systemConfig = GetSystemConfig(systemId);
        var handler = await GetHandler(systemConfig);
        return await handler.GetPlayCodeAsync(systemId, locationId, code);
    }

    public async Task<PlayCodeData> CreatePlayCodeAsync(int systemId, int locationId, decimal amount)
    {
        var systemConfig = GetSystemConfig(systemId);
        var handler = await GetHandler(systemConfig);
        return await handler.CreatePlayCodeAsync(systemId, locationId, amount);
    }

    public async Task<CashoutResult> CashOutPlayCodeAsync(int systemId, int locationId, string code)
    {
        var systemConfig = GetSystemConfig(systemId);
        var handler = await GetHandler(systemConfig);
        return await handler.CashOutPlayCodeAsync(systemId, locationId, code);
    }

    public async Task<PlayCodeData> TopUpPlayCodeAsync(int systemId, int locationId, string code, decimal amount)
    {
        var systemConfig = GetSystemConfig(systemId);
        var handler = await GetHandler(systemConfig);
        return await handler.TopUpPlayCodeAsync(systemId, locationId, code, amount);
    }
    
    #region Helpers

    private SystemConfig GetSystemConfig(int systemId)
    {
        var systemConfig = _options.Value.Systems.SingleOrDefault(s => s.SystemId == systemId);
        if (systemConfig == null)
            throw new SystemNotFoundException(systemId, string.Empty);
        return systemConfig;
    }
    
    private async Task<IProxyPlayCodeApiHandler> GetHandler(SystemConfig systemConfig)
    {
        IProxyPlayCodeApiHandler? handler;

        if (systemConfig.Type == "okbingo")
            handler = _okBingoApiHandler;
        else if (systemConfig.Type == "http")
            handler = _httpHandler;
        else
            throw new HandlerNotFoundException($"System type '{systemConfig.Type}' is not implemented.");

        await handler.ConfigureAsync(systemConfig.Connection);

        return handler;
    }
    
    #endregion
}