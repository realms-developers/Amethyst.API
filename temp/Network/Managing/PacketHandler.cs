namespace Amethyst.Network.Managing;

public delegate void PacketHandler<T>(in T packet, PacketHandleResult result);