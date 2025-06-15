using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Chat.Base.Misc.Context;

public record PlayerTeamMessageContext(PlayerEntity Player, byte TeamID);
