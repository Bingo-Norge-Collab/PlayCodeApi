namespace PlayCodeApi.Domain;

public class PlayCode
{
    /// <summary>
    /// The unique identifier for this playcode
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Future proofing for multi-currency support
    /// </summary>
    public string Currency { get; set; } = "NOK";

    /// <summary>
    /// The current amount available for play
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Has the code been cashed out? A cashed out code is considered "complete" and cannot be used again.
    /// </summary>
    public bool IsCashedOut { get; set; }
    
    /// <summary>
    /// The date and time when the code is no longer valid (for play, can still be cashed out).
    /// </summary>
    public DateTime ValidUntil { get; set; }
}