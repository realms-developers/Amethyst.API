using Amethyst.Storages.Mongo;

namespace Amethyst.Gameplay.Players.Auth;

public sealed class PlayerAccountModel(string name) : DataModel(name)
{
    public override void Save()
    {
    }

    public override void Remove()
    {
    }
}
