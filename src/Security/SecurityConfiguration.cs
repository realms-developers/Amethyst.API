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
}
