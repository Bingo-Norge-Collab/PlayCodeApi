namespace PlayCodeApi.Proxy.ApiHandler;

public class OkBingoReceipt
{
    public int UniqueIdentifier { get; set; }
    public string TicketNumber { get; set; }
    public decimal Kr { get; set; }
    public decimal NewSaldo { get; set; }
    public DateTime ExpirationDate { get; set; }
    public OkBingoStatusCode StatusCode { get; set; }
    public string ErrorText { get; set; } = string.Empty;
}