namespace Amethyst.Commands;

public enum CommandType
{
    /// <summary>
    /// Can used be by everyone who have permission.
    /// </summary>
    Shared,

    /// <summary>
    /// Can be used by console or RCON.
    /// </summary>
    Console,

    /// <summary>
    /// Can be used in developer server mode.
    /// </summary>
    Debug
}