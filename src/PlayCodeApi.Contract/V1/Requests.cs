namespace PlayCodeApi.Contract.V1;

/// <summary>
/// Purchase a new playcode with the given amount 
/// </summary>
/// <param name="Amount">Amount to initialize playcode with</param>
public record PlayCodePurchase(decimal Amount);

/// <summary>
/// Add funds to an existing playcode
/// </summary>
/// <param name="Amount">The amount to add to playcode</param>
public record PlayCodeTopUp(decimal Amount);