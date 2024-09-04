using AutoMapper;
using PlayCodeApi.Application;
using PlayCodeApi.Contract.V1;
using PlayCodeApi.Domain;
using PlayCodeApi.PlayCodes;

namespace PlayCodeApi.SwaggerDemo.PlayCodes;

/// <summary>
/// In memory repository implementation. This is a demo implementation and should not be used in production.
/// </summary>
public class DemoPlayCodeApiHandler : IPlayCodeApiHandler
{
    private readonly IMapper _mapper;
    private int _nextId = 100_001;
    private Dictionary<string, PlayCode> _playCodes = new();

    public DemoPlayCodeApiHandler(IMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public Task<PlayCodeData?> GetPlayCodeAsync(string code)
    {
        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, code);
        
        var data = _mapper.Map<PlayCodeData?>(playCode);
        return Task.FromResult(data);
    }

    /// <inheritdoc />
    public Task<PlayCodeData> CreatePlayCodeAsync(decimal amount)
    {
        Guard.AgainstInvalidAmount(amount);
        
        var playCode = new PlayCode() { Id = _nextId++, Amount = amount, ValidUntil = DateTime.Now + Globals.PlayCodeDuration };
        _playCodes.Add(playCode.Id.ToString(), playCode);
        
        var data = _mapper.Map<PlayCodeData>(playCode);
        return Task.FromResult(data);
    }
    
    /// <inheritdoc />
    public Task<CashoutResult> CashOutPlayCodeAsync(string code)
    {
        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, code);
        Guard.AgainstCashedOut(playCode!, code);

        var amount = playCode!.Amount;
        playCode.Amount = 0;
        playCode.IsCashedOut = true;
        
        var data = _mapper.Map<PlayCodeData>(playCode);
        return Task.FromResult(new CashoutResult(amount, data));
    }

    /// <inheritdoc />
    public Task<PlayCodeData> TopUpPlayCodeAsync(string code, decimal amount)
    {
        Guard.AgainstInvalidAmount(amount);

        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, code);
        Guard.AgainstCashedOut(playCode!, code);
        Guard.AgainstExpired(playCode!, code);
        
        playCode!.Amount += amount;
        
        var data = _mapper.Map<PlayCodeData>(playCode);
        return Task.FromResult(data);
    }
}

/// <summary>
/// Assumed validations. Your mileage may vary.
/// </summary>
public static class Guard
{
    public static void AgainstInvalidAmount(decimal amount)
    {
        if (amount <= 0)
            throw new InvalidAmountException();
    }

    public static void AgainstNotFound(PlayCode? playCode, string code)
    {
        if (playCode == null)
            throw new PlayCodeNotFoundException(code);
    }

    public static void AgainstCashedOut(PlayCode playCode, string code)
    {
        if (playCode.IsCashedOut)
            throw new PlayCodeAlreadyCashedOutException(code);
    }

    public static void AgainstExpired(PlayCode playCode, string code)
    {
        if (playCode.ValidUntil < DateTime.Now)
            throw new PlayCodeExpiredException(code);
    }
}