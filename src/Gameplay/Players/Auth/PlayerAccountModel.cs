using Amethyst.Storages.Mongo;

namespace Amethyst.Players.Auth;

public sealed class PlayerAccountModel : DataModel
{
    public PlayerAccountModel(string name) : base(name)
    {
    }

    public override void Save()
    {
    }

    public override void Remove()
    {
    }
}
