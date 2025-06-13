using Amethyst.Network.Enums;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;

namespace Amethyst.Network.Handling.Packets.Platform;

public sealed class PlatformHandler : INetworkHandler
{
    public string Name => "net.amethyst.PlatformHandler";

    public void Load()
    {
        NetworkManager.AddDirectHandler(150, OnPlatformPacket);
    }

    private void OnPlatformPacket(PlayerEntity plr, ReadOnlySpan<byte> data, ref bool ignore)
    {
        FastPacketReader reader = new(data, 3);
        byte platform = reader.ReadByte();
        plr.PlatformType = (PlatformType)platform;
    }

    public void Unload()
    {
        NetworkManager.RemoveDirectHandler(150, OnPlatformPacket);
    }
}
