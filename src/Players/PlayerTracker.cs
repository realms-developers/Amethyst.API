using System.Collections;

namespace Amethyst.Players;

public sealed class PlayerTracker : IEnumerable<NetPlayer>
{
    private readonly NetPlayer[] _players = new NetPlayer[256];

    public IEnumerable<NetPlayer> Capable => _players.Where(p => p != null && p.IsActive && p.IsCapable);

    public IEnumerable<NetPlayer> Alive => _players.Where(p => p != null && p.IsActive && !p.TPlayer.dead);
    public IEnumerable<NetPlayer> Dead => _players.Where(p => p != null && p.IsActive && p.TPlayer.dead);

    public NetPlayer this[int index] => _players[index];

    public void CreateInstance(int index)
    {
        NetPlayer player = new(index);
        _players[index] = player;
    }

    public NetPlayer? FindPlayer(string nameOrIndex)
    {
        bool isIndex = int.TryParse(nameOrIndex, out int index);

        NetPlayer? fullNameSearch = _players.FirstOrDefault(p => p != null && p.IsActive && p.Name.Equals(nameOrIndex, StringComparison.OrdinalIgnoreCase));
        if (fullNameSearch != null)
        {
            return fullNameSearch;
        }

        if (isIndex)
        {
            NetPlayer indexSearch = _players[index];
            if (indexSearch != null && indexSearch.IsActive)
            {
                return indexSearch;
            }
        }

        return _players.FirstOrDefault(p => p != null && p.IsActive && p.Name.StartsWith(nameOrIndex, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerator<NetPlayer> GetEnumerator() => _players.Where(p => p != null).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
