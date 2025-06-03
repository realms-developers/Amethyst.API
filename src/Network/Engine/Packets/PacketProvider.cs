using Amethyst.Server.Entities.Players;
using Amethyst.Network.Engine.Delegates;
using System.Reflection.Emit;
using System.Reflection;
using Amethyst.Network.Packets;

namespace Amethyst.Network.Engine.Packets;

internal sealed class PacketProvider<TPacket>
{
    internal List<PacketHook<TPacket>> _securityHandlers = new List<PacketHook<TPacket>>();
    internal List<PacketHook<TPacket>> _handlers = new List<PacketHook<TPacket>>();
    internal PacketHook<TPacket>? _mainHandler;
    internal bool _wasHooked;
    internal IPacket<TPacket> _packet = null!;
    internal Type _packetType = null!;

    internal PacketHook<TPacket>[] _ivkSecurityHandlers = [];
    internal PacketHook<TPacket>[] _ivkHandlers = [];

    internal Func<ReadOnlySpan<byte>, int, TPacket> _deserializeFunc = null!;

    public void Hookup()
    {
        if (_wasHooked)
            return;

        _wasHooked = true;

        _packetType = typeof(TPacket).Assembly.GetType($"{typeof(TPacket).FullName}Packet")!;
        _packet = (IPacket<TPacket>)Activator.CreateInstance(_packetType)!;

        _deserializeFunc = (Func<ReadOnlySpan<byte>, int, TPacket>)Delegate.CreateDelegate(
            typeof(Func<ReadOnlySpan<byte>, int, TPacket>),
            _packetType.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static)!);

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

        _ivkHandlers = _handlers.ToArray();
    }
    internal void UnregisterHandler(PacketHook<TPacket> handler)
    {
        _handlers.Remove(handler);

        _ivkHandlers = _handlers.ToArray();
    }

    internal void RegisterSecurityHandler(PacketHook<TPacket> handler, int priority = 0)
    {
        if (_securityHandlers.Contains(handler))
            return;

        _securityHandlers.Add(handler);
        _securityHandlers.Sort((x, y) => priority.CompareTo(0));

        _ivkSecurityHandlers = _securityHandlers.ToArray();
    }
    internal void UnregisterSecurityHandler(PacketHook<TPacket> handler)
    {
        _securityHandlers.Remove(handler);

        _ivkSecurityHandlers = _securityHandlers.ToArray();
    }

    internal void Invoke(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    {
        var packet = _deserializeFunc(data, 0);

        for (int i = 0; i < _ivkSecurityHandlers.Length; i++)
        {
            _ivkSecurityHandlers[i](plr, ref packet, data, ref ignore);
            if (ignore)
                return;
        }

        for (int i = 0; i < _ivkHandlers.Length; i++)
        {
            _ivkHandlers[i](plr, ref packet, data, ref ignore);
        }

        if (ignore)
            return;

        _mainHandler?.Invoke(plr, ref packet, data, ref ignore);
    }
}
