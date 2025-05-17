using Amethyst.Core;
using Microsoft.Xna.Framework;

namespace Amethyst.Players;

public sealed class PlayerJail
{
    internal PlayerJail(NetPlayer player)
    {
        BasePlayer = player;
    }

    public bool IsJailed => IsJailForced || JailExpiration > DateTime.UtcNow || _jailDelegates.Any(p => p(BasePlayer));

    public bool IsJailForced { get; private set; }
    public DateTime JailExpiration { get; private set; }
    public IReadOnlyList<JailCheck> JailDelegates => _jailDelegates.AsReadOnly();

    public NetPlayer BasePlayer { get; private set; }

    private List<JailCheck> _jailDelegates = new List<JailCheck>();

    public void AddCheck(JailCheck checkDelegate)
    {
        if (_jailDelegates.Contains(checkDelegate))
        {
            return;
        }

        _jailDelegates.Add(checkDelegate);
    }

    public void RemoveCheck(JailCheck checkDelegate)
    {
        _jailDelegates.Remove(checkDelegate);
    }

    public void SetForce(bool value = true)
    {
        IsJailForced = value;

        if (AmethystSession.Profile.DebugMode)
            BasePlayer.SendMessage($"[Debug]: Jail -> Force {value}.", Color.OrangeRed);
    }

    public void SetTemp(TimeSpan span)
    {
        DateTime newExpiration = DateTime.UtcNow + span;

        if (JailExpiration > newExpiration)
        {
            return;
        }

        JailExpiration = newExpiration;
        BasePlayer.Utils.Disable(span);

        if (AmethystSession.Profile.DebugMode)
            BasePlayer.SendMessage($"[Debug]: Jail -> Temp {span.TotalSeconds}s.", Color.OrangeRed);
    }

    public delegate bool JailCheck(NetPlayer player);
}
