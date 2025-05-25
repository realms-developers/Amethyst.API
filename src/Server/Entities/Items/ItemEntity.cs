using Amethyst.Server.Entities.Base;
using Terraria;

namespace Amethyst.Server.Entities.Items;

public sealed class ItemEntity : IServerEntity
{
    public ItemEntity(int index)
    {
        Index = index;
    }

    public Item TItem => Main.item[Index];

    public int Index { get; }

    public bool Active => TItem.active;

    public string Name => TItem.Name;
}
