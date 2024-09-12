namespace PlayCodeApi.Proxy.ApiHandler;

public class OkBingoReceipt
{
    public int UniqueIdentifier { get; set; }
    public int TicketNumber { get; set; }
    public decimal Kr { get; set; }
    public decimal NewSaldo { get; set; }
    public DateTime ExpirationDate { get; set; }
    public int ErrorNumber { get; set; }
    public string ErrorText { get; set; } = string.Empty;
}