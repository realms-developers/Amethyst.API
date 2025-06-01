using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Core.Delegates;

internal delegate void PacketInvokeHandler(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore);
