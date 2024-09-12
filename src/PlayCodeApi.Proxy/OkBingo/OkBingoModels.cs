// ReSharper disable InconsistentNaming

using System.ComponentModel.DataAnnotations;

namespace PlayCodeApi.Proxy.OkBingo;

public static class OkBingoCommandCode
{
    public const int MakeTicket = 1; // UniqueIDentifier;TicketNumber;Kr;Print
    public const int AddToTicket = 2; // UniqueIDentifier;TicketNumber;Kr;Print
    public const int CloseTickete = 3; // UniqueIDentifier;TicketNumber;Kr;Print
    public const int GetSaldoOnTicket = 5; // UniqueIDentifier;TicketNumber
    
    public const int ReceiptMakeTicket = 101; // UniqueIDentifier;TicketNumber;Kr;NewSaldo;ExperationDate;ErrorNumber;ErrorDescriptionToUser
    public const int ReceiptAddToTicket = 102; // UniqueIDentifier;TicketNumber;Kr;NewSaldo;ExperationDate;ErrorNumber;ErrorDescriptionToUser
    public const int ReceiptCloseTickete = 103; // UniqueIDentifier;TicketNumber;Kr;NewSaldo;ExperationDate;ErrorNumber;ErrorDescriptionToUser
    public const int ReceiptGetSaldoOnTicket = 105; // UniqueIDentifier;TicketNumber;Kr;NewSaldo;ExperationDate;ErrorNumber;ErrorDescriptionToUser
    
    public const int CloseDay = 10;
    public const int OpenDay = 11;
    
    public const int ReceiptCloseDay = 110;
    public const int ReceiptOpenDay = 111;
}

public class OkBingoCommand
{
    public OkBingoCommand()
    {
    }
    
    public static OkBingoCommand Create(int bingoId, int systemId, int comandId, 
        int uniqueIdentifier, string ticketNumber, decimal amount, bool print) 
        => new()
        {
            BingoID = bingoId,
            FromSystemID = 0,
            ToSystemID = systemId,
            ComandID = comandId,
            Parameter = $"{uniqueIdentifier};{ticketNumber};{(amount > 0 ? $"{amount}" : "")};{(print ? "1" : "0")}",
        };

    [Key]
    public int ComID { get; set; }

    public int BingoID { get; set; }
    public int FromSystemID { get; set; }
    public int ToSystemID { get; set; }
    // Intentional typo to match DB
    public int ComandID { get; set; }
    public string Parameter { get; set; } = null!;
    public DateTime TimeStamp { get; set; }
}