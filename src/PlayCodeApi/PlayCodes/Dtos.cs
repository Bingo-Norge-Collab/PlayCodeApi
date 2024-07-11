namespace PlayCodeApi.PlayCodes;

// Inbound

/// <summary>
/// Purchase a new playcode with the given amount 
/// </summary>
/// <param name="Amount">Amount to initialize playcode with</param>
record PlayCodePurchase(decimal Amount);

/// <summary>
/// Add funds to an existing playcode
/// </summary>
/// <param name="Amount">The amount to add to playcode</param>
record PlayCodeTopUp(decimal Amount);

// Outbound
/// <summary>
/// Result of a cashout operation
/// </summary>
/// <param name="Amount">The amount that was cashed out from playcode</param>
/// <param name="PlayCode">The updated playcode after operation</param>
public record CashoutResult(decimal Amount, PlayCode PlayCode);