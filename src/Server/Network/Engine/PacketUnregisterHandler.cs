using Amethyst.Server.Network.Engine.Packets;

namespace Amethyst.Server.Network.Engine;

internal delegate void PacketUnregisterHandler<TPacket>(PacketHook<TPacket> hook);
