using System.Collections;
using Terraria;

namespace Amethyst.Gameplay.Items;

public sealed class ItemTracker : IEnumerable<NetWorldItem>
{
    public NetWorldItem? this[int index]
    {
        get
        {
            if (index < 0 || index >= Main.item.Length)
            {
                return null;
            }

            Item? item = Main.item[index];
            if (item == null || !item.active)
            {
                return null;
            }

            return new NetWorldItem(item.whoAmI);
        }
    }

    public IEnumerator<NetWorldItem> GetEnumerator()
    {
        return Main.item.Where(p => p != null && p.active).Select(p => new NetWorldItem(p.whoAmI)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
