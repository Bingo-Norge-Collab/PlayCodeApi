using PlayCodeApi.Application;

namespace PlayCodeApi.Proxy.ApiHandler;

public interface IProxyPlayCodeApiHandler : IPlayCodeApiHandler
{
    Task ConfigureAsync(string connection);
}