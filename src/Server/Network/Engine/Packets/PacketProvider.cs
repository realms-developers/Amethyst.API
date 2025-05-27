using Amethyst.Server.Entities.Players;

namespace Amethyst.Server.Network.Engine.Packets;

internal sealed class PacketProvider<TPacket>
{
    internal List<PacketHook<TPacket>> _handlers = new List<PacketHook<TPacket>>();
    internal IPacket<TPacket> _packet = Activator.CreateInstance<IPacket<TPacket>>();
    internal bool _wasHooked;

    public void Hookup()
    {
        if (_wasHooked)
            return;

        _wasHooked = true;

        NetworkManager.RegisterHandlers.Add(typeof(TPacket), new PacketRegisterHandler<TPacket>(RegisterHandler));
        NetworkManager.UnregisterHandlers.Add(typeof(TPacket), new PacketUnregisterHandler<TPacket>(UnregisterHandler));
        NetworkManager.InvokeHandlers[(byte)_packet.PacketID] = Invoke;
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

    internal void Invoke(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    {
        if (_handlers.Count == 0)
            return;

        var packet = _packet.Deserialize(data);

        foreach (var handler in _handlers)
        {
            handler(plr, packet, ref ignore);
        }
    }
}
