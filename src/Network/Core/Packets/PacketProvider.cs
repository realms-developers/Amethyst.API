using Amethyst.Server.Entities.Players;
using Amethyst.Network.Core.Delegates;

namespace Amethyst.Network.Core.Packets;

internal sealed class PacketProvider<TPacket>
{
    internal List<PacketHook<TPacket>> _securityHandlers = new List<PacketHook<TPacket>>();
    internal List<PacketHook<TPacket>> _handlers = new List<PacketHook<TPacket>>();
    internal PacketHook<TPacket>? _mainHandler;
    internal IPacket<TPacket> _packet = Activator.CreateInstance<IPacket<TPacket>>();
    internal bool _wasHooked;

    public void Hookup()
    {
        if (_wasHooked)
            return;

        _wasHooked = true;

        NetworkManager.InvokeHandlers[(byte)_packet.PacketID] = Invoke;
    }

    internal void SetMainHandler(PacketHook<TPacket>? hook)
    {
        _mainHandler = hook;
    }

    internal void RegisterHandler(PacketHook<TPacket> handler, int priority = 0)
    {
        if (_handlers.Contains(handler))
            return;

        _handlers.Add(handler);
        _handlers.Sort((x, y) => priority.CompareTo(0));
    }
    internal void UnregisterHandler(PacketHook<TPacket> handler)
    {
        _handlers.Remove(handler);
    }

    internal void RegisterSecurityHandler(PacketHook<TPacket> handler, int priority = 0)
    {
        if (_securityHandlers.Contains(handler))
            return;

        _securityHandlers.Add(handler);
        _securityHandlers.Sort((x, y) => priority.CompareTo(0));
    }
    internal void UnregisterSecurityHandler(PacketHook<TPacket> handler)
    {
        _securityHandlers.Remove(handler);
    }

    internal void Invoke(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    {
        if (_handlers.Count == 0)
            return;

        var packet = _packet.Deserialize(data);

        foreach (var securityHandler in _securityHandlers)
        {
            securityHandler(plr, ref packet, data, ref ignore);
            if (ignore)
                return;
        }

        foreach (var handler in _handlers)
        {
            handler(plr, ref packet, data, ref ignore);
        }

        _mainHandler?.Invoke(plr, ref packet, data, ref ignore);
    }
}
