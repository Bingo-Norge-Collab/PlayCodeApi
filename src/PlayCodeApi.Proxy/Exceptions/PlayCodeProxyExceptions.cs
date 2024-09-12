using System.Net;

namespace PlayCodeApi.Proxy.Exceptions;

public class PlayCodeProxyException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public class HandlerNotFoundException(string handler) 
    : PlayCodeProxyException($"Request handler for type: {handler} not found.", HttpStatusCode.NotFound);
    
public class OkBingoException(string error) 
    : PlayCodeProxyException($"Ok bingo request failed: {error}.", HttpStatusCode.InternalServerError);