namespace Amethyst.Gameplay.Players.Auth;

public class AuthConfiguration
{
    public bool EnableAuthorization { get; set; }

    public int? MinPasswordLength { get; set; } = 6;
    public int? MaxPasswordLength { get; set; } = 48;
}
