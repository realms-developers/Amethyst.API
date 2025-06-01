#pragma warning disable CA1051

using Terraria.DataStructures;

namespace Amethyst.Server.Network.Structures;

public struct NetDeathReason
{
    public static NetDeathReason CreateEmpty()
        => new NetDeathReason() { SourceOtherIndex = 254 };

    public static NetDeathReason CreateDefault()
        => new NetDeathReason() { SourceOtherIndex = 255 };

    public NetDeathReason()
    {
        SourcePlayerIndex = -1;
        SourceNPCIndex = -1;
        SourceProjectileLocalIndex = -1;
        SourceOtherIndex = -1;
        SourceProjectileType = 0;
        SourceItemType = 0;
        SourceItemPrefix = 0;
        SourceCustomReason = null; ;
    }

    public NetDeathReason(int sourcePlayerIndex, int sourceNPCIndex, int sourceProjectileLocalIndex, int sourceOtherIndex, int sourceProjectileType, int sourceItemType, int sourceItemPrefix, string? sourceCustomReason)
    {
        SourcePlayerIndex = sourcePlayerIndex;
        SourceNPCIndex = sourceNPCIndex;
        SourceProjectileLocalIndex = sourceProjectileLocalIndex;
        SourceOtherIndex = sourceOtherIndex;
        SourceProjectileType = sourceProjectileType;
        SourceItemType = sourceItemType;
        SourceItemPrefix = sourceItemPrefix;
        SourceCustomReason = sourceCustomReason;
    }

    public static implicit operator PlayerDeathReason(NetDeathReason netDeathReason)
    {
        return new PlayerDeathReason
        {
            _sourcePlayerIndex = netDeathReason.SourcePlayerIndex,
            _sourceNPCIndex = netDeathReason.SourceNPCIndex,
            _sourceProjectileLocalIndex = netDeathReason.SourceProjectileLocalIndex,
            _sourceOtherIndex = netDeathReason.SourceOtherIndex,
            _sourceProjectileType = netDeathReason.SourceProjectileType,
            _sourceItemType = netDeathReason.SourceItemType,
            _sourceItemPrefix = netDeathReason.SourceItemPrefix,
            _sourceCustomReason = netDeathReason.SourceCustomReason ?? string.Empty
        };
    }

    public static implicit operator NetDeathReason(PlayerDeathReason playerDeathReason)
    {
        return new NetDeathReason
        {
            SourcePlayerIndex = playerDeathReason._sourcePlayerIndex,
            SourceNPCIndex = playerDeathReason._sourceNPCIndex,
            SourceProjectileLocalIndex = playerDeathReason._sourceProjectileLocalIndex,
            SourceOtherIndex = playerDeathReason._sourceOtherIndex,
            SourceProjectileType = playerDeathReason._sourceProjectileType,
            SourceItemType = playerDeathReason._sourceItemType,
            SourceItemPrefix = playerDeathReason._sourceItemPrefix,
            SourceCustomReason = string.IsNullOrEmpty(playerDeathReason._sourceCustomReason) ? null : playerDeathReason._sourceCustomReason
        };
    }

    public int SourcePlayerIndex;

    public int SourceNPCIndex;

    public int SourceProjectileLocalIndex;

    public int SourceOtherIndex;

    public int SourceProjectileType;

    public int SourceItemType;

    public int SourceItemPrefix;

    public string? SourceCustomReason;
}
