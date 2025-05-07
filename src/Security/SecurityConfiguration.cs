namespace Amethyst.Security;

public struct SecurityConfiguration
{
    public bool DisableSecurity { get; set; }
    public List<string> DisabledRules { get; set; }

    public Dictionary<int, int> PerSecondLimitPackets { get; set; }
    public List<int> OneTimePackets { get; set; }
    public List<int> DisabledPackets { get; set; }


    public Dictionary<int, int> PerSecondLimitModules { get; set; }
    public List<int> OneTimeModules { get; set; }
    public List<int> DisabledModules { get; set; }

    public bool? NotifyModerators { get; set; }

    public bool? EnableNicknameFilter { get; set; }
    public string NicknameFilter { get; set; }

    public bool PreventStackCheat { get; set; }

    public int? MaxAllowedLife { get; set; }
    public int? MaxAllowedMana { get; set; }

    public int? KillTileRange { get; set; }
    public int? PlaceTileRange { get; set; }
    public int? ReplaceTileRange { get; set; }

    public int? KillTileThreshold { get; set; }
    public int? PlaceTileThreshold { get; set; }
    public int? ReplaceTileThreshold { get; set; }

    public int? KillWallRange { get; set; }
    public int? PlaceWallRange { get; set; }
    public int? ReplaceWallRange { get; set; }

    public int? KillWallThreshold { get; set; }
    public int? PlaceWallThreshold { get; set; }
    public int? ReplaceWallThreshold { get; set; }

    public bool DisableItemDropThreshold { get; set; }
    public int? ItemDropThreshold { get; set; }
    public bool? ReturnDroppedItemInThreshold { get; set; }

    public List<string> AllowedMessages { get; set; }
}
