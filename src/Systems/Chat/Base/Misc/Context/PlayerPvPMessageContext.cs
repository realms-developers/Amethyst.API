using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Chat.Base.Misc.Context;

public record PlayerPvPMessageContext(PlayerEntity Player, bool Value);
