namespace Amethyst.Players.Auth;

public struct AuthConfiguration
{
    public bool EnableAuthorization { get; set; }

    public int? MinPasswordLength { get; set; }
    public int? MaxPasswordLength { get; set; }
}
