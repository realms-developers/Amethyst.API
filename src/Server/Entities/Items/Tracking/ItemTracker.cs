using System.Collections;
using Amethyst.Server.Entities.Base;
using Terraria;

namespace Amethyst.Server.Entities.Items.Tracking;

public sealed class ItemTracker : IEntityTracker<ItemEntity>
{
    public ItemTracker()
    {
    }

    public IEntityManager<ItemEntity>? Manager => null;
    public ItemEntity this[int index] => new(index);

    public IEnumerator<ItemEntity> GetEnumerator()
    {
        return Main.item.Where(p => p != null && p.active && p.type > 0 && p.stack > 0)
            .Select((item, index) => new ItemEntity(index))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
