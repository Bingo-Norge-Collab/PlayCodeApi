using PlayCodeApi.Contract.V1;
using PlayCodeApi.Proxy.ApiHandler;

namespace PlayCodeApi.Proxy.OkBingo;

public interface IOkBingoApiHandler : IProxyPlayCodeApiHandler { }

public class OkBingoApiHandler : IOkBingoApiHandler
{
    private readonly IOkBingoService _okBingoService;

    public OkBingoApiHandler(IOkBingoService okBingoService)
    {
        _okBingoService = okBingoService;
    }

    public Task ConfigureAsync(string connectionString)
    {
        _okBingoService.ConnectionString = connectionString;
        return Task.CompletedTask;
    }

    public async Task<PlayCodeData?> GetPlayCodeAsync(int systemId, int locationId, string code)
    {
        var receipt = await _okBingoService.GetSaldoOnTicketAsync(systemId, locationId, code);
        return GetPlayCodeData(receipt);
    }

    public async Task<PlayCodeData> CreatePlayCodeAsync(int systemId, int locationId, decimal amount)
    {
        var receipt = await _okBingoService.MakeTicketAsync(systemId, locationId, amount);
        return GetPlayCodeData(receipt);
    }

    public async Task<CashoutResult> CashOutPlayCodeAsync(int systemId, int locationId, string code)
    {
        var receipt = await _okBingoService.CloseTicketAsync(systemId, locationId, code);
        var data = GetPlayCodeData(receipt);
        
        // For close ticket receipt, the remaining amount is stored in the Kr field
        return new CashoutResult(receipt.Kr, data);
    }

    public async Task<PlayCodeData> TopUpPlayCodeAsync(int systemId, int locationId, string code, decimal amount)
    {
        var receipt = await _okBingoService.AddToTicketAsync(systemId, locationId, amount, code);
        return GetPlayCodeData(receipt);
    }

    #region Helpers
    private PlayCodeData GetPlayCodeData(OkBingoReceipt receipt)
    {
        return new PlayCodeData
        {
            Id = receipt.TicketNumber,
            Amount = receipt.NewSaldo,
            ValidUntil = receipt.ExpirationDate,
            Currency = "NOK",
            IsCashedOut = receipt.NewSaldo == 0
        };
    }
    #endregion
}