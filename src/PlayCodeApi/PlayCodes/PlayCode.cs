namespace PlayCodeApi.PlayCodes;

public class PlayCode
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public bool IsCashedOut { get; set; }
    public DateTime ValidUntil { get; set; }
}