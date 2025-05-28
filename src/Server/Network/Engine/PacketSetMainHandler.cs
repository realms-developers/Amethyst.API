using Amethyst.Server.Network.Engine.Packets;

namespace Amethyst.Server.Network.Engine;

internal delegate void PacketSetMainHandler<TPacket>(PacketHook<TPacket>? hook);
