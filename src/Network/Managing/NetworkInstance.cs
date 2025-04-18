using Amethyst.Network.Packets;
using Terraria.ID;

namespace Amethyst.Network.Managing;

public sealed class NetworkInstance
{
    internal NetworkInstance()
    {
        for (int i = 0; i < Incoming.Length; i++)
        {
            Incoming[i] = new List<PacketHandler<IncomingPacket>>(16);
        }

        for (int i = 0; i < IncomingModules.Length; i++)
        {
            IncomingModules[i] = new List<PacketHandler<IncomingModule>>(16);
        }

        for (int i = 0; i < Outcoming.Length; i++)
        {
            Outcoming[i] = new List<PacketHandler<OutcomingPacket>>(16);
        }
    }

    public List<PacketHandler<IncomingPacket>>[] Incoming { get; } = new List<PacketHandler<IncomingPacket>>[MessageID.Count];
    public List<PacketHandler<IncomingModule>>[] IncomingModules { get; } = new List<PacketHandler<IncomingModule>>[255];
    public List<PacketHandler<OutcomingPacket>>[] Outcoming { get; } = new List<PacketHandler<OutcomingPacket>>[MessageID.Count];

    public PacketHandler<IncomingPacket>?[] IncomingReplace { get; } = new PacketHandler<IncomingPacket>?[MessageID.Count];
    public PacketHandler<IncomingModule>?[] IncomingModuleReplace { get; } = new PacketHandler<IncomingModule>?[255];
    public PacketHandler<OutcomingPacket>?[] OutcomingReplace { get; } = new PacketHandler<OutcomingPacket>?[MessageID.Count];
}
