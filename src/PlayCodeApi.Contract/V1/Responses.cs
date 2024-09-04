namespace PlayCodeApi.Contract.V1;

/// <summary>
/// Result of a cashout operation
/// </summary>
/// <param name="Amount">The amount that was cashed out from playcode</param>
/// <param name="PlayCode">The updated playcode after operation</param>
public record CashoutResult(decimal Amount, PlayCodeData PlayCode);