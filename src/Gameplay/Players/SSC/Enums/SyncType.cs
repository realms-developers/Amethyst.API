namespace Amethyst.Gameplay.Players.SSC.Enums;

public enum SyncType
{
    /// <summary>
    /// Syncing only with character owner. 
    /// </summary>
    Local,

    /// <summary>
    /// Syncing exclude character owner.
    /// </summary>
    Exclude,

    /// <summary>
    /// Syncing with all players.
    /// </summary>
    Broadcast
}