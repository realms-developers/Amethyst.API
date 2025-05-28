using Amethyst.Server.Entities.Players;

namespace Amethyst.Server.Network.Engine.Packets;

public delegate void PacketHook<TPacket>(PlayerEntity plr, TPacket packet, ref bool ignore);
