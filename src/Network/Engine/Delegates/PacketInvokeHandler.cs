using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Engine.Delegates;

public delegate void PacketInvokeHandler(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore);
