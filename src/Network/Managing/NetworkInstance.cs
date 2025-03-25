using Amethyst.Network.Packets;
using Terraria.ID;

namespace Amethyst.Network.Managing;

public sealed class NetworkInstance
{
    internal NetworkInstance()
    {
        for (int i = 0; i < incoming.Length; i++)
            incoming[i] = new List<PacketHandler<IncomingPacket>>(16);

        for (int i = 0; i < incomingModules.Length; i++)
            incomingModules[i] = new List<PacketHandler<IncomingModule>>(16);

        for (int i = 0; i < outcoming.Length; i++)
            outcoming[i] = new List<PacketHandler<OutcomingPacket>>(16);
    }

    public List<PacketHandler<IncomingPacket>>[] incoming = new List<PacketHandler<IncomingPacket>>[MessageID.Count];
    public List<PacketHandler<IncomingModule>>[] incomingModules = new List<PacketHandler<IncomingModule>>[255];
    public List<PacketHandler<OutcomingPacket>>[] outcoming = new List<PacketHandler<OutcomingPacket>>[MessageID.Count];

    public PacketHandler<IncomingPacket>?[] incomingReplace = new PacketHandler<IncomingPacket>?[MessageID.Count];
    public PacketHandler<IncomingModule>?[] incomingModuleReplace = new PacketHandler<IncomingModule>?[255];
    public PacketHandler<OutcomingPacket>?[] outcomingReplace = new PacketHandler<OutcomingPacket>?[MessageID.Count];
}