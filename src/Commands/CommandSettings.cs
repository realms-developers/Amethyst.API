namespace Amethyst.Commands;

[Flags]
public enum CommandSettings
{
    /// <summary>
    /// Executed only in-game.
    /// </summary>
    IngameOnly = 1,

    /// <summary>
    /// Suppress execution logging.
    /// </summary>
    HideLog = 2,

    /// <summary>
    /// Exclude from help listings.
    /// </summary>
    Hidden = 4
}
