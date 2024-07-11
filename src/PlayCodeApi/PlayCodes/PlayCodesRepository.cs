namespace PlayCodeApi.PlayCodes;

public interface IPlayCodesRepository
{
    /// <summary>
    /// Get a play code by its unique code
    /// </summary>
    /// <param name="code">The unique code for this playcode</param>
    /// <returns>PlayCode object</returns>
    /// <throws>PlayCodeNotFoundException</throws>
    Task<PlayCode?> GetPlayCodeAsync(string code);
    
    /// <summary>
    /// Create a playcode with a given amount
    /// </summary>
    /// <param name="amount">Starting amount for playcode</param>
    /// <returns>PlayCode object</returns>
    /// <throws>InvalidAmountException</throws>
    Task<PlayCode> CreatePlayCodeAsync(decimal amount);
    
    /// <summary>
    /// Cashout playcode. The playcode is considered "closed" and cannot be used again.
    /// </summary>
    /// <param name="code">The unique code for this playcode</param>
    /// <returns>Result of operation. Contains playcode and amount that was cashed out</returns>
    /// <throws>PlayCodeNotFoundException</throws>
    /// <throws>PlayCodeAlreadyCashedOutException</throws>
    Task<CashoutResult> CashOutPlayCodeAsync(string code);

    /// <summary>
    /// Top up a playcode with a given amount
    /// </summary>
    /// <param name="code">The unique code for this playcode</param>
    /// <param name="amount">The amount to add</param>
    /// <returns>The updated playcode</returns>
    /// <throws>InvalidAmountException</throws>
    /// <throws>PlayCodeNotFoundException</throws>
    /// <throws>PlayCodeAlreadyCashedOutException</throws>
    /// <throws>PlayCodeExpiredException</throws>
    Task<PlayCode> TopUpPlayCodeAsync(string code, decimal amount);
}

/// <summary>
/// In memory repository implementation.
/// This would probably be replaced with a database or API in a real-world scenario.
/// </summary>
public class InMemoryPlayCodesRepository : IPlayCodesRepository
{
    private int _nextId = 100_001;
    private Dictionary<string, PlayCode> _playCodes = new();
    
    /// <inheritdoc />
    public Task<PlayCode?> GetPlayCodeAsync(string code)
    {
        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, code);
        
        return Task.FromResult(playCode)!;
    }

    /// <inheritdoc />
    public Task<PlayCode> CreatePlayCodeAsync(decimal amount)
    {
        Guard.AgainstInvalidAmount(amount);
        
        var playCode = new PlayCode() { Id = _nextId++, Amount = amount, ValidUntil = DateTime.Now + Globals.PlayCodeDuration };
        _playCodes.Add(playCode.Id.ToString(), playCode);
        return Task.FromResult(playCode);
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
        return Task.FromResult(new CashoutResult(amount, playCode));
    }

    /// <inheritdoc />
    public Task<PlayCode> TopUpPlayCodeAsync(string code, decimal amount)
    {
        Guard.AgainstInvalidAmount(amount);

        _playCodes.TryGetValue(code, out var playCode);
        Guard.AgainstNotFound(playCode, code);
        Guard.AgainstCashedOut(playCode!, code);
        Guard.AgainstExpired(playCode!, code);
        
        playCode!.Amount += amount;
        return Task.FromResult(playCode);
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