using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Chat.Misc.Context;

public record PlayerTeamMessageContext(PlayerEntity Player, byte TeamID);
