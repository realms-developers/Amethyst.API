namespace Amethyst.Players.Auth;

public sealed class PlayerAuth
{
    public NetPlayer Player { get; }
    public bool IsAuthorized => _factors.All(p => p.Value);
    public IReadOnlyDictionary<string, bool> Factors => _factors;

    private readonly Dictionary<string, bool> _factors;
    internal PlayerAuth(NetPlayer player, IEnumerable<string> factors)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));

        if(factors == null || !factors.Any())
            throw new ArgumentException("Factors cannot be empty", nameof(factors));

        _factors = factors.ToDictionary(f => f, _ => false);
    }

    public void SwitchFactor(string factorName, bool value)
    {
        if (_factors.ContainsKey(factorName))
        {
            _factors[factorName] = value;
        }
    }
}
