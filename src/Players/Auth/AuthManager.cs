using Amethyst.Core;

namespace Amethyst.Players.Auth;

public static class AuthManager
{
    public static IReadOnlyList<string> Factors => _Factors.AsReadOnly();
    public static AuthConfiguration Configuration => AmethystSession.Profile.Config.Get<AuthConfiguration>().Data;

    internal static List<string> _Factors = [ "password" ];

    internal static void Initialize()
    {
        AmethystSession.Profile.Config.Get<AuthConfiguration>().Load();
        AmethystSession.Profile.Config.Get<AuthConfiguration>().Modify(SetupConfiguration, true);
    }

    private static void SetupConfiguration(ref AuthConfiguration configuration)
    {
        configuration.MinPasswordLength ??= 6;
        configuration.MaxPasswordLength ??= 48;
    }

    public static void AddAuthFactor(string name)
    {
        if (!_Factors.Contains(name))
        {
            _Factors.Add(name);
        }
    }

    public static void RemoveAuthFactor(string name)
    {
        _Factors.Remove(name);
    }
}
