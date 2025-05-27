using Amethyst.Server.Network.Engine.Packets;

namespace Amethyst.Server.Network.Engine;

internal delegate void PacketRegisterHandler<TPacket>(PacketHook<TPacket> hook, int priority = 0);
