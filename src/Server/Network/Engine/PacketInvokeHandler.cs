using Amethyst.Server.Entities.Players;

namespace Amethyst.Server.Network.Engine;

internal delegate void PacketInvokeHandler(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore);
