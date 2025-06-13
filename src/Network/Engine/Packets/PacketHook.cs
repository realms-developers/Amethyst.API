using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Engine.Packets;

public delegate void PacketHook<TPacket>(PlayerEntity plr, ref TPacket packet, ReadOnlySpan<byte> rawPacket, ref bool ignore);
