using AutoMapper;
using PlayCodeApi.Application;
using PlayCodeApi.Contract.V1;
using PlayCodeApi.Domain;

namespace PlayCodeApi.SwaggerDemo.ApiHandler;

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
    public Task<PlayCodeData?> GetPlayCodeAsync(int systemId, int locationId, string code)
    {
        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, systemId, code);
        
        var data = _mapper.Map<PlayCodeData?>(playCode);
        return Task.FromResult(data);
    }

    /// <inheritdoc />
    public Task<PlayCodeData> CreatePlayCodeAsync(int systemId, int locationId, decimal amount)
    {
        Guard.AgainstInvalidAmount(amount, systemId, string.Empty);
        
        var playCode = new PlayCode() { Id = _nextId++, Amount = amount, ValidUntil = DateTime.Now + Globals.PlayCodeDuration };
        _playCodes.Add(playCode.Id.ToString(), playCode);
        
        var data = _mapper.Map<PlayCodeData>(playCode);
        return Task.FromResult(data);
    }
    
    /// <inheritdoc />
    public Task<CashoutResult> CashOutPlayCodeAsync(int systemId, int locationId, string code)
    {
        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, systemId, code);
        Guard.AgainstCashedOut(playCode!, systemId, code);

        var amount = playCode!.Amount;
        playCode.Amount = 0;
        playCode.IsCashedOut = true;
        
        var data = _mapper.Map<PlayCodeData>(playCode);
        return Task.FromResult(new CashoutResult(amount, data));
    }

    /// <inheritdoc />
    public Task<PlayCodeData> TopUpPlayCodeAsync(int systemId, int locationId, string code, decimal amount)
    {
        Guard.AgainstInvalidAmount(amount, systemId, code);

        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, systemId, code);
        Guard.AgainstCashedOut(playCode!, systemId, code);
        Guard.AgainstExpired(playCode!, systemId, code);
        
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
    public static void AgainstInvalidAmount(decimal amount, int systemId, string code)
    {
        if (amount <= 0)
            throw new InvalidAmountException(systemId, code);
    }

    public static void AgainstNotFound(PlayCode? playCode, int systemId, string code)
    {
        if (playCode == null)
            throw new PlayCodeNotFoundException(systemId, code);
    }

    public static void AgainstCashedOut(PlayCode playCode, int systemId, string code)
    {
        if (playCode.IsCashedOut)
            throw new PlayCodeAlreadyCashedOutException(systemId, code);
    }

    public static void AgainstExpired(PlayCode playCode, int systemId, string code)
    {
        if (playCode.ValidUntil < DateTime.Now)
            throw new PlayCodeExpiredException(systemId, code);
    }
}