namespace Amethyst.Players;

public sealed class PlayerJail
{
    public bool IsJailed => IsJailForced || JailExpiration > DateTime.UtcNow;

    public bool IsJailForced { get; private set; }
    public DateTime JailExpiration { get; private set; }

    public void ForceJail(bool value = true)
    {
        IsJailForced = value;
    }

    public void TempJail(TimeSpan span)
    {
        DateTime newExpiration = DateTime.UtcNow + span;

        if (JailExpiration > newExpiration)
        {
            return;
        }

        JailExpiration = newExpiration;
    }
}
