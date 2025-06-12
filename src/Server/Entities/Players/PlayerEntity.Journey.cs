using Amethyst.Network.Enums;
using Amethyst.Network.Utilities;

namespace Amethyst.Server.Entities.Players;

public sealed partial class PlayerEntity
{
    public bool IsJourneyMode => Difficulty == 3;

    public ref bool IsGodModeEnabled => ref TPlayer.creativeGodMode;

    public void SetGodMode(bool enabled)
    {
        FastPacketWriter writer = new(82, new byte[64]);
        writer.WriteUInt16((ushort)ModuleID.JourneyPowers);
        writer.WriteUInt16(5);
        writer.WriteByte(1);
        writer.WriteByte((byte)Index);
        writer.WriteBoolean(enabled);
        SendPacketBytes(writer.Build());
        IsGodModeEnabled = enabled;
    }

    public void ResearchItem(short itemType, short amount = 1)
    {
        FastPacketWriter writer = new(82, new byte[64]);
        writer.WriteUInt16((ushort)ModuleID.JourneyServersideResearch);
        writer.WriteInt16(itemType);
        writer.WriteInt16(amount);
        SendPacketBytes(writer.Build());
    }
}
