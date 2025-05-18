using Amethyst.Storages.Config;

namespace Amethyst.Gameplay.Players.Auth;

public static class AuthManager
{
    internal static readonly Configuration<AuthConfiguration> _authCfg = new(typeof(AuthConfiguration).FullName!, new());

    public static IReadOnlyList<string> Factors => _Factors.AsReadOnly();
    public static AuthConfiguration Configuration => _authCfg.Data;

    internal static List<string> _Factors = ["password"];

    public static void Initialize() => _authCfg.Load();

    public static void AddAuthFactor(string name)
    {
        if (!_Factors.Contains(name))
        {
            _Factors.Add(name);
        }
    }

    public static void RemoveAuthFactor(string name) => _Factors.Remove(name);
}
