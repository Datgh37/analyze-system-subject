namespace ExpenseTracker.Domain.Enums;

// Types of behavioral nudges inspired by Richard Thaler Nudge Theory (UC-09.01)
public enum NudgeType
{
    DsaThreshold = 1,        // Spending near or exceeding daily spending allowance (DSA)
    ZombieSubscription = 2,  // Inactive recurring subscription draining funds without usage
    PayYourselfFirst = 3,    // Savings prompt when new income arrives
    DeficitRebalance = 4     // Dynamic budget rebalancing recommendation
}

// Lifecycle status of a financial nudge (UC-09.04)
public enum NudgeStatus
{
    Active = 1,     // Active nudge waiting for user review
    Resolved = 2,   // Approved and applied by user
    Dismissed = 3   // Dismissed or ignored by user
}

// Status of a cooling-off period transaction (UC-09.05)
public enum CoolingOffStatus
{
    PendingCoolingOff = 1, // Within 24h-48h cooling-off window (wallet funds not yet deducted)
    Confirmed = 2,         // User confirmed purchase after cooldown expired
    Cancelled = 3,         // User cancelled purchase and saved money
    Bypassed = 4           // Emergency bypass with user justification
}
