using PlayCodeApi.Contract.V1;

namespace PlayCodeApi.Application;

public interface IPlayCodeApiHandler
{
    /// <summary>
    /// Get a play code by its unique code
    /// </summary>
    /// <param name="systemId">The SystemID. 0 = Master.</param>
    /// <param name="locationId">The Location ID. AKA BingoID or HallID.</param>
    /// <param name="code">The unique code for this playcode</param>
    /// <returns>PlayCode object</returns>
    /// <throws>PlayCodeNotFoundException</throws>
    /// <throws>PlayCodeInUseException</throws>
    Task<PlayCodeData?> GetPlayCodeAsync(int systemId, int locationId, string code);
    
    /// <summary>
    /// Create a playcode with a given amount
    /// </summary>
    /// <param name="systemId">The SystemID. 0 = Master.</param>
    /// <param name="locationId">The Location ID. AKA BingoID or HallID.</param>
    /// <param name="amount">Starting amount for playcode</param>
    /// <returns>PlayCode object</returns>
    /// <throws>InvalidAmountException</throws>
    Task<PlayCodeData> CreatePlayCodeAsync(int systemId, int locationId, decimal amount);
    
    /// <summary>
    /// Cashout playcode. The playcode is considered "closed" and cannot be used again.
    /// </summary>
    /// <param name="systemId">The SystemID. 0 = Master.</param>
    /// <param name="locationId">The Location ID. AKA BingoID or HallID.</param>
    /// <param name="code">The unique code for this playcode</param>
    /// <returns>Result of operation. Contains playcode and amount that was cashed out</returns>
    /// <throws>PlayCodeNotFoundException</throws>
    /// <throws>PlayCodeAlreadyCashedOutException</throws>
    /// <throws>PlayCodeInUseException</throws>
    Task<CashoutResult> CashOutPlayCodeAsync(int systemId, int locationId, string code);

    /// <summary>
    /// Top up a playcode with a given amount
    /// </summary>
    /// <param name="systemId">The SystemID. 0 = Master.</param>
    /// <param name="locationId">The Location ID. AKA BingoID or HallID.</param>
    /// <param name="code">The unique code for this playcode</param>
    /// <param name="amount">The amount to add</param>
    /// <returns>The updated playcode</returns>
    /// <throws>InvalidAmountException</throws>
    /// <throws>PlayCodeNotFoundException</throws>
    /// <throws>PlayCodeAlreadyCashedOutException</throws>
    /// <throws>PlayCodeExpiredException</throws>
    /// <throws>PlayCodeInUseException</throws>
    Task<PlayCodeData> TopUpPlayCodeAsync(int systemId, int locationId, string code, decimal amount);
}