using Amethyst.Network.Structures;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Systems.Chat.Base.Models;

public record MessageRenderResult(
    PlayerEntity Entity,
    IReadOnlyDictionary<string, string> Prefix,
    IReadOnlyDictionary<string, string> Name,
    IReadOnlyDictionary<string, string> Suffix,
    IReadOnlyDictionary<string, string> Text,
    NetColor Color);
