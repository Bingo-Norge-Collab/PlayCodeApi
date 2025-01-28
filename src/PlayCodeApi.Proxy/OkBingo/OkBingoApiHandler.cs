using System.Net;
using PlayCodeApi.Contract.V1;
using PlayCodeApi.Proxy.ApiHandler;

namespace PlayCodeApi.Proxy.OkBingo;

public interface IOkBingoApiHandler : IProxyPlayCodeApiHandler { }

public class OkBingoApiHandler : IOkBingoApiHandler
{
    private readonly IOkBingoService _okBingoService;
    private readonly ILogger<OkBingoApiHandler> _logger;

    public OkBingoApiHandler(IOkBingoService okBingoService, ILogger<OkBingoApiHandler> logger)
    {
        _okBingoService = okBingoService;
        _logger = logger;
    }

    public Task ConfigureAsync(string connectionString)
    {
        _okBingoService.ConnectionString = connectionString;
        return Task.CompletedTask;
    }

    public async Task<PlayCodeData?> GetPlayCodeAsync(int systemId, int locationId, string code)
    {
        var receipt = await _okBingoService.GetSaldoOnTicketAsync(systemId, locationId, code);
        HandleErrorCode(receipt, systemId, string.Empty);
        
        return GetPlayCodeData(receipt);
    }

    public async Task<PlayCodeData> CreatePlayCodeAsync(int systemId, int locationId, decimal amount)
    {
        var receipt = await _okBingoService.MakeTicketAsync(systemId, locationId, amount);
        HandleErrorCode(receipt, systemId, string.Empty);
        
        return GetPlayCodeData(receipt);
    }

    public async Task<CashoutResult> CashOutPlayCodeAsync(int systemId, int locationId, string code)
    {
        var receipt = await _okBingoService.CloseTicketAsync(systemId, locationId, code);
        HandleErrorCode(receipt, systemId, string.Empty);
        var data = GetPlayCodeData(receipt);

        // For close ticket receipt, the remaining amount is stored in the Kr field
        return new CashoutResult(receipt.Kr, data);
    }

    public async Task<PlayCodeData> TopUpPlayCodeAsync(int systemId, int locationId, string code, decimal amount)
    {
        var receipt = await _okBingoService.AddToTicketAsync(systemId, locationId, amount, code);
        HandleErrorCode(receipt, systemId, string.Empty);
        
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
            //IsCashedOut = receipt.NewSaldo == 0 // This is wrong
        };
    }

    private void HandleErrorCode(OkBingoReceipt receipt, int systemId, string code)
    {
        if (receipt.StatusCode != OkBingoStatusCode.Success)
        {
            _logger.LogError("Ok Bingo Command failed - {StatusCode}: {ErrorText}", receipt.StatusCode, receipt.ErrorText);
            
            switch (receipt.StatusCode)
            {
                case OkBingoStatusCode.InvalidAmountForPurchase:
                case OkBingoStatusCode.InvalidAmountForTopUp:
                case OkBingoStatusCode.ValueCannotBeZero:
                case OkBingoStatusCode.ValueGreaterThanLimitInCAS:
                    throw new InvalidAmountException(systemId, receipt.TicketNumber.ToString());
                
                case OkBingoStatusCode.InvalidTicketNumberForTopUp:
                case OkBingoStatusCode.InvalidTicketNumberForCloseTicket:
                case OkBingoStatusCode.InvalidTicket:
                case OkBingoStatusCode.TicketNotFound:
                    throw new PlayCodeNotFoundException(systemId, receipt.TicketNumber.ToString());
                
                case OkBingoStatusCode.TicketAlreadyClosed:
                case OkBingoStatusCode.TicketIsClosed:
                case OkBingoStatusCode.TicketIsExpired:
                    throw new PlayCodeAlreadyCashedOutException(systemId, receipt.TicketNumber.ToString());
                
                case OkBingoStatusCode.TicketIsInUse:
                    throw new PlayCodeInUseException(systemId, receipt.TicketNumber.ToString());
                
                case OkBingoStatusCode.LSNotRegisteredInCAS:
                case OkBingoStatusCode.PSKeyNotRegistered:
                case OkBingoStatusCode.InvalidPSKey:
                case OkBingoStatusCode.InvalidCASDate:
                case OkBingoStatusCode.OperationFailed:
                case OkBingoStatusCode.CASIsClosed:
                case OkBingoStatusCode.PSKeyIsBlocked:
                default:
                    throw new SystemFaultException(receipt.ErrorText, systemId, receipt.TicketNumber.ToString());
            }
        }
    }

    #endregion
}