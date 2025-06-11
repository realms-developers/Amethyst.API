using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Terraria;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.Signs;

public sealed class SignsHandler : INetworkHandler
{
    public string Name => "net.amethyst.SignsHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<SignRead>(OnSignRead);
        NetworkManager.SetMainHandler<SignSync>(OnSignSync);
    }

    private void OnSignSync(PlayerEntity plr, ref SignSync packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        if (packet.SignIndex < 0 || packet.SignIndex >= Main.sign.Length)
        {
            ignore = true;
            return;
        }

        bool sendPacket = Main.sign[packet.SignIndex]?.text != packet.SignText;

        Sign newSign = new Sign
        {
            x = packet.SignX,
            y = packet.SignY,
            text = packet.SignText
        };

        Main.sign[packet.SignIndex] = newSign;
        Sign.TextSign(packet.SignIndex, packet.SignText);

        if (sendPacket)
        {
            NetMessage.TrySendData(47, -1, plr.Index, NetworkText.Empty, packet.SignIndex, plr.Index);
        }
    }

    private void OnSignRead(PlayerEntity plr, ref SignRead packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
            return;

        int signIndex = Sign.ReadSign(packet.SignX, packet.SignY);

        if (signIndex >= 0)
        {
            NetMessage.TrySendData(47, plr.Index, -1, NetworkText.Empty, signIndex, plr.Index);
        }
    }


    public void Unload()
    {
        NetworkManager.SetMainHandler<SignRead>(null);
        NetworkManager.SetMainHandler<SignSync>(null);
    }
}
