using Terraria.Localization;

namespace Amethyst.Network.Packets;

public sealed class OutcomingPacket(byte packetId, int remote, int ignore, NetworkText? text, int num1, float num2, float num3,
    float num4, int num5, int num6, int num7) : IPacket
{
    public byte PacketID { get; } = packetId;

    public int RemoteClient { get; set; } = remote;
    public int IgnoreClient { get; set; } = ignore;
    public NetworkText? Text { get; set; } = text;
    public int Number1 { get; set; } = num1;
    public float Number2 { get; set; } = num2;
    public float Number3 { get; set; } = num3;
    public float Number4 { get; set; } = num4;
    public int Number5 { get; set; } = num5;
    public int Number6 { get; set; } = num6;
    public int Number7 { get; set; } = num7;
}
