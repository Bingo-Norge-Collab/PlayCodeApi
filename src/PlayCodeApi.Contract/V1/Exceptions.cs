using System;
using System.Net;

namespace PlayCodeApi.Contract.V1;

public static class PlayCodeErrorCodes
{
    public const int PlayCodeNotFound = 1001;
    public const int SystemFault = 1002;
    public const int SystemNotFound = 1003;
    public const int InvalidAmount = 1004;
    public const int PlayCodeAlreadyCashedOut = 1005;
    public const int PlayCodeExpired = 1006;
    public const int PlayCodeInUse = 1007;
    public const int SerializationIssue = 1008;
    public const int NoContent = 204;
    public const int UnknownError = 500;
}

public class PlayCodeException(string message, HttpStatusCode statusCode, int errorCode, int systemId, string playCode) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
    public int ErrorCode { get; } = errorCode;
    public int SystemId { get; } = systemId;
    public string PlayCode { get; } = playCode;

    public static void ThrowFromErrorCode(int errorCode, int systemId, string playCode, string customMessage = "")
    {
        switch (errorCode)
        {
            case PlayCodeErrorCodes.PlayCodeNotFound: throw new PlayCodeNotFoundException(systemId, playCode);
            case PlayCodeErrorCodes.SystemFault: throw new SystemFaultException(customMessage, systemId, playCode);
            case PlayCodeErrorCodes.SystemNotFound: throw new SystemNotFoundException(systemId, playCode);
            case PlayCodeErrorCodes.InvalidAmount: throw new InvalidAmountException(systemId, playCode);
            case PlayCodeErrorCodes.PlayCodeAlreadyCashedOut: throw new PlayCodeAlreadyCashedOutException(systemId, playCode);
            case PlayCodeErrorCodes.PlayCodeExpired: throw new PlayCodeExpiredException(systemId, playCode);
            case PlayCodeErrorCodes.PlayCodeInUse: throw new PlayCodeInUseException(systemId, playCode);
        }
    }
}

public class PlayCodeNotFoundException(int systemId, string playCode) : PlayCodeException($"Play code {playCode} not found.",
    HttpStatusCode.NotFound, PlayCodeErrorCodes.PlayCodeNotFound, systemId, playCode);

public class SystemFaultException(string message, int systemId, string playCode) 
    : PlayCodeException(message, HttpStatusCode.InternalServerError, PlayCodeErrorCodes.SystemFault, systemId, playCode);
public class SystemNotFoundException(int systemId, string playCode) 
    : PlayCodeException($"System with ID={systemId} not found.", HttpStatusCode.NotFound, PlayCodeErrorCodes.SystemNotFound, systemId, playCode);

public class InvalidAmountException(int systemId, string playCode) 
    : PlayCodeException($"Amount should be greater than 0", HttpStatusCode.BadRequest, PlayCodeErrorCodes.InvalidAmount, systemId, playCode);

public class PlayCodeAlreadyCashedOutException(int systemId, string playCode) 
    : PlayCodeException($"Play code {playCode} has already been cashed out.", HttpStatusCode.BadRequest, PlayCodeErrorCodes.PlayCodeAlreadyCashedOut, systemId, playCode);
    
public class PlayCodeExpiredException(int systemId, string playCode)
    : PlayCodeException($"Play code {playCode} has expired.", HttpStatusCode.BadRequest, PlayCodeErrorCodes.PlayCodeExpired, systemId, playCode);
    
public class PlayCodeInUseException(int systemId, string playCode)
    : PlayCodeException($"Play code {playCode} is in use.", HttpStatusCode.Conflict, PlayCodeErrorCodes.PlayCodeInUse, systemId, playCode);