using Amethyst.Server.Entities.Players;

namespace Amethyst.Server.Network.Core.Packets;

public delegate void PacketHook<TPacket>(PlayerEntity plr, ref TPacket packet, ReadOnlySpan<byte> rawPacket, ref bool ignore);
