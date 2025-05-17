namespace Amethyst.Players.Auth;

public sealed class PlayerAuth
{
    internal PlayerAuth(NetPlayer player)
    {
        Player = player;

        _factors = new Dictionary<string, bool>();

        List<string> factors = AuthManager._Factors;
        foreach (string factor in factors)
        {
            _factors.Add(factor, false);
        }
    }

    private Dictionary<string, bool> _factors;

    public NetPlayer Player { get; }

    public bool IsAuthorized => _factors.All(p => p.Value);

    public void SwitchFactor(string factorName, bool value)
    {
        if (_factors.ContainsKey(factorName))
        {
            _factors[factorName] = value;
        }
    }
}
