using System.Globalization;
using Amethyst.Storages.Mongo;

namespace Amethyst.Security.GameBans;

public sealed class GameObjectBlocker
{
    internal GameObjectBlocker(string name)
    {
        models = new MongoModels<GameObjectBan>(MongoDatabase.Main, name);
        bans = models.FindAll().Select(static p => int.Parse(p.Name, CultureInfo.CurrentCulture)).ToHashSet();
    }

    internal MongoModels<GameObjectBan> models;
    internal HashSet<int> bans;

    public void Refresh()
    {
        bans = models.FindAll().Select(static p => int.Parse(p.Name, CultureInfo.CurrentCulture)).ToHashSet();
    }

    public void Add(int id)
    {
        bans.Add(id);
        models.Save(new GameObjectBan(id.ToString(CultureInfo.CurrentCulture)));
    }

    public void Remove(int id)
    {
        bans.Remove(id);
        models.Remove(id.ToString(CultureInfo.CurrentCulture));
    }

    public void Contains(int id) => bans.Contains(id);

    public IEnumerable<int> GetEnumerable()
    {
        foreach (int value in bans)
            yield return value;
    }
}
