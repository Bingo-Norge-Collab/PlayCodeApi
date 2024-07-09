namespace PlayCodeApi.PlayCodes;

public interface IPlayCodesRepository
{
    Task<PlayCode?> GetPlayCodeAsync(string code);
    Task<PlayCode> CreatePlayCodeAsync(decimal amount);
    Task<decimal> CashOutPlayCodeAsync(string code);
    Task<PlayCode> TopUpPlayCodeAsync(string code, decimal amount);
}

public class InMemoryPlayCodesRepository : IPlayCodesRepository
{
    private int _nextId = 100_001;
    private Dictionary<string, PlayCode> _playCodes = new();
    
    public Task<PlayCode?> GetPlayCodeAsync(string code)
    {
        if (!_playCodes.TryGetValue(code, out var playCode))
            throw new ArgumentException("Invalid play code");
        
        return Task.FromResult(playCode)!;
    }

    public Task<PlayCode> CreatePlayCodeAsync(decimal amount)
    {
        var playCode = new PlayCode() { Id = _nextId++, Amount = amount, ValidUntil = DateTime.Now + TimeSpan.FromDays(3) };
        _playCodes.Add(playCode.Id.ToString(), playCode);
        return Task.FromResult(playCode);
    }
    
    public Task<decimal> CashOutPlayCodeAsync(string code)
    {
        if (!_playCodes.TryGetValue(code, out var playCode))
            throw new ArgumentException("Invalid play code");

        var amount = playCode.Amount;
        playCode.Amount = 0;
        playCode.IsCashedOut = true;
        return Task.FromResult(amount);
    }

    public Task<PlayCode> TopUpPlayCodeAsync(string code, decimal amount)
    {
        if (!_playCodes.TryGetValue(code, out var playCode))
            throw new ArgumentException("Invalid play code");
        
        playCode.Amount += amount;
        return Task.FromResult(playCode);
    }
}