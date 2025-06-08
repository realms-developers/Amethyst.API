using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Chat.Misc.Context;

public record PlayerPvPMessageContext(PlayerEntity Player, bool Value);
