using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Network.Packets;
using Microsoft.Xna.Framework;

namespace Amethyst.Security.Rules.World;

public sealed class UpdateItemRule : ISecurityRule
{
    public string Name => "coresec_itemUpdate";

    public void Load(NetworkInstance net)
    {
        net.SecureIncoming[21].Add(OnUpdateItem);
        net.SecureIncoming[90].Add(OnUpdateItem);
        net.SecureIncoming[145].Add(OnUpdateItem);
        net.SecureIncoming[148].Add(OnUpdateItem);
    }

    private bool OnUpdateItem(in IncomingPacket packet)
    {
        var reader = packet.GetReader();

        int index = reader.ReadInt16();
	    Vector2 position = reader.ReadVector2();
		Vector2 velocity = reader.ReadVector2();
		int stack = reader.ReadInt16();
		int prefix = reader.ReadByte();
		int ownIgnore = reader.ReadByte();
		int type = reader.ReadInt16();

        return false;
    }

    public void Unload(NetworkInstance net)
    {
        net.SecureIncoming[21].Remove(OnUpdateItem);
        net.SecureIncoming[90].Remove(OnUpdateItem);
        net.SecureIncoming[145].Remove(OnUpdateItem);
        net.SecureIncoming[148].Remove(OnUpdateItem);
    }
}
