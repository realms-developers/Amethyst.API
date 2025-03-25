namespace Amethyst.Commands;

[Flags]
public enum CommandSettings
{
    /// <summary>
    /// Command with that setting can be executed only in game.
    /// </summary>
    IngameOnly = 1,

    /// <summary>
    /// Command with that setting does not log into console.
    /// </summary>
    HideLog = 2,

    /// <summary>
    /// Command with that setting does not shows in '/help commands'
    /// </summary>
    Hidden = 4
}