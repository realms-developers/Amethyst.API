using Amethyst.Network;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;

namespace Amethyst.Items;

public sealed class NetWorldItem
{
    internal NetWorldItem(int index)
    {
        Index = index;
    }

    public int Index { get; }
    public Item TItem => Main.item[Index];

    public void Modify(NetItem item, bool networkSync = true)
    {
        TItem.type = item.ID;
        TItem.stack = item.Stack;
        TItem.prefix = item.Prefix;

        if (networkSync)
        {
            NetworkSync();
        }
    }

    public void Modify(int itemId, short itemStack, byte itemPrefix = 0, bool networkSync = true)
    {
        TItem.type = itemId;
        TItem.stack = itemStack;
        TItem.prefix = itemPrefix;

        if (networkSync)
        {
            NetworkSync();
        }
    }

    public void ModifyPosition(Vector2 value, bool networkSync = true)
    {
        TItem.position = value;

        if (networkSync)
        {
            NetworkSync();
        }
    }

    public void ModifyVelocity(Vector2 value, bool networkSync = true)
    {
        TItem.velocity = value;

        if (networkSync)
        {
            NetworkSync();
        }
    }

    public void Remove(bool networkSync = true)
    {
        TItem.active = false;
        TItem.type = 0;
        TItem.stack = 0;

        if (networkSync)
        {
            NetworkSync();
        }
    }

    public void NetworkSync(byte ownIgnore = 0, int remoteClient = -1, int ignoreClient = -1)
    {
        NetMessage.SendData(21, remoteClient, ignoreClient, NetworkText.Empty, Index, ownIgnore);
    }
}
