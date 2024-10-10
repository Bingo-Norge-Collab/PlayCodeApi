using System.Net;
using Microsoft.EntityFrameworkCore;
using PlayCodeApi.Proxy.ApiHandler;
using PlayCodeApi.Proxy.Data;
using PlayCodeApi.Proxy.Exceptions;

namespace PlayCodeApi.Proxy.OkBingo;

public interface IOkBingoService
{
    Task<OkBingoReceipt> MakeTicketAsync(int systemId, int locationId, decimal amount, string ticketNumber = "", bool print = false);
    Task<OkBingoReceipt> AddToTicketAsync(int systemId, int locationId, decimal amount, string ticketNumber, bool print = false);
    Task<OkBingoReceipt> CloseTicketAsync(int systemId, int locationId, string ticketNumber, bool print = false);
    Task<OkBingoReceipt> GetSaldoOnTicketAsync(int systemId, int locationId, string ticketNumber);
    string ConnectionString { get; set; }
}

public class OkBingoService : IOkBingoService
{
    public static int MessageNumber = 1;

    public string ConnectionString { get; set; }

    public async Task<OkBingoReceipt> MakeTicketAsync(int systemId, int locationId, decimal amount, 
        string ticketNumber = "", bool print = false)
    {
        var cmd = OkBingoCommand.Create(locationId, systemId, OkBingoCommandCode.MakeTicket, 
            MessageNumber++, ticketNumber, amount, print);
        
        var receipt = await SendCommand(cmd, OkBingoCommandCode.ReceiptMakeTicket);
        return receipt;
    }

    public async Task<OkBingoReceipt> AddToTicketAsync(int systemId, int locationId, decimal amount, 
        string ticketNumber, bool print = false)
    {
        var cmd = OkBingoCommand.Create(locationId, systemId, OkBingoCommandCode.AddToTicket, 
            MessageNumber++, ticketNumber, amount, print);
        
        var receipt = await SendCommand(cmd, OkBingoCommandCode.ReceiptAddToTicket);
        return receipt;
    }
    
    public async Task<OkBingoReceipt> CloseTicketAsync(int systemId, int locationId, 
        string ticketNumber, bool print = false)
    {
        var cmd = OkBingoCommand.Create(locationId, systemId, OkBingoCommandCode.CloseTickete, 
            MessageNumber++, ticketNumber, 0, print);
        
        var receipt = await SendCommand(cmd, OkBingoCommandCode.ReceiptCloseTickete);
        return receipt;
    }
    
    public async Task<OkBingoReceipt> GetSaldoOnTicketAsync(int systemId, int locationId, string ticketNumber)
    {
        var cmd = OkBingoCommand.Create(locationId, systemId, OkBingoCommandCode.GetSaldoOnTicket, 
            MessageNumber++, ticketNumber, 0, false);
        
        var receipt = await SendCommand(cmd, OkBingoCommandCode.ReceiptGetSaldoOnTicket);
        return receipt;
    }
    
    #region Helpers

    private async Task<OkBingoReceipt> SendCommand(OkBingoCommand cmd, int receiptCommandType)
    {
        OkBingoReceipt? receipt = null;
        
        var optionsBuilder = new DbContextOptionsBuilder<OkBingoDbContext>();
        optionsBuilder.UseSqlServer(ConnectionString);

        await using var context = new OkBingoDbContext(optionsBuilder.Options);

        try
        {
            // Need to set timestamp to something, otherwise sql server throws an error 
            cmd.TimeStamp = DateTime.Now;
            
            await context.Commands.AddAsync(cmd);
            await context.SaveChangesAsync();

            receipt = await GetReceipt(context, cmd.ToSystemID, cmd.BingoID, cmd.ComID, receiptCommandType);
        } 
        catch (Exception ex)
        {
            throw new PlayCodeProxyException($"DB Query failed: {ex.Message}", HttpStatusCode.InternalServerError);
        }
        
        if (receipt == null)
            throw new OkBingoException($"Did not receive receipt for command {cmd.ComandID} (ID={cmd.ComID})");

        return receipt;
    }
    
    private async Task<OkBingoReceipt?> GetReceipt(OkBingoDbContext ctx, int systemId, int locationId, int commandId, 
        int commandType)
    {
        OkBingoCommand? receipt;
        
        int retries = 10;
        do
        {
            await Task.Delay(500);
            receipt = await ctx.Commands.FirstOrDefaultAsync(r
                => r.ComID > commandId
                   && r.ComandID == commandType
                   && r.BingoID == locationId
                   && r.FromSystemID == systemId);
            
            retries--;
        } while (receipt == null && retries > 0);
        
        return receipt == null ? null : CreateReceipt(receipt);
    }
    
    private OkBingoReceipt CreateReceipt(OkBingoCommand receipt)
    {
        var parts = receipt.Parameter.Split(";");
        var uniqueIdentifier = parts.Length > 0 ? parts[0] : string.Empty;
        var ticketNumber = parts.Length > 1 ? parts[1] : string.Empty;
        var amount = parts.Length > 2 ? parts[2] : string.Empty;
        var newSaldo = parts.Length > 3 ? parts[3] : string.Empty;
        var expirationDate = parts.Length > 4 ? parts[4] : string.Empty;
        var errorNumber = parts.Length > 5 ? parts[5] : string.Empty;
        var errorText = parts.Length > 6 ? parts[6] : string.Empty;

        return new OkBingoReceipt()
        {
            UniqueIdentifier = string.IsNullOrWhiteSpace(uniqueIdentifier) ? 0 : int.Parse(uniqueIdentifier),
            TicketNumber = string.IsNullOrWhiteSpace(ticketNumber) ? 0 : int.Parse(ticketNumber),
            Kr = string.IsNullOrWhiteSpace(amount) ? 0 : decimal.Parse(amount),
            NewSaldo = string.IsNullOrWhiteSpace(newSaldo) ? 0 : decimal.Parse(newSaldo),
            ExpirationDate = string.IsNullOrWhiteSpace(expirationDate) ? DateTime.MaxValue : DateTime.Parse(expirationDate),
            StatusCode = string.IsNullOrWhiteSpace(errorNumber) ? OkBingoStatusCode.Success : (OkBingoStatusCode)int.Parse(errorNumber),
            ErrorText = errorText,
        };
    }
    
    #endregion
}