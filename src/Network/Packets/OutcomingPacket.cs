using Terraria.Localization;

namespace Amethyst.Network.Packets;

public sealed class OutcomingPacket : IPacket
{
    public OutcomingPacket(byte packetId, int remote, int ignore, NetworkText? text, int num1, float num2, float num3,
        float num4, int num5, int num6, int num7)
    {
        PacketID = packetId;
        RemoteClient = remote;
        IgnoreClient = ignore;
        Text = text;
        Number1 = num1;
        Number2 = num2;
        Number3 = num3;
        Number4 = num4;
        Number5 = num5;
        Number6 = num6;
        Number7 = num7;
    }


    public byte PacketID { get; }

    public int RemoteClient;
    public int IgnoreClient;
    public NetworkText? Text;
    public int Number1;
    public float Number2;
    public float Number3;
    public float Number4;
    public int Number5;
    public int Number6;
    public int Number7;
}