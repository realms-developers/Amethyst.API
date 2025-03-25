namespace Amethyst.Network.Managing;

public delegate void PacketHandler<T>(T packet, PacketHandleResult result);