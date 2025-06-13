namespace Amethyst.Systems.Users.Players;

public record class PlayerUserMetadata(
    string Name,
    string IP,
    string UUID,
    int NetIndex);
