using System;
using System.Net;

namespace PlayCodeApi.PlayCodes;

public class PlayCodeException(string message, HttpStatusCode statusCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}

public class PlayCodeNotFoundException(string playCode) 
    : PlayCodeException($"Play code {playCode} not found.", HttpStatusCode.NotFound);

public class SystemNotFoundException(int systemId) 
    : PlayCodeException($"System with ID={systemId} not found.", HttpStatusCode.NotFound);

public class InvalidAmountException() 
    : PlayCodeException($"Amount should be greater than 0", HttpStatusCode.BadRequest);

public class PlayCodeAlreadyCashedOutException(string playCode) 
    : PlayCodeException($"Play code {playCode} has already been cashed out.", HttpStatusCode.BadRequest);
    
public class PlayCodeExpiredException(string playCode)
    : PlayCodeException($"Play code {playCode} has expired.", HttpStatusCode.BadRequest);