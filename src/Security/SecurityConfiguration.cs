namespace Amethyst.Security;

public class SecurityConfiguration
{
    public bool DisableSecurity { get; set; }
    public List<string> DisabledRules { get; set; } = [];

    public Dictionary<int, int> PerSecondLimitPackets { get; set; } = [];
    public List<int> OneTimePackets { get; set; } = [1, 6, 8];
    public List<int> DisabledPackets { get; set; } = [136];

    public Dictionary<int, int> PerSecondLimitModules { get; set; } = [];
    public List<int> OneTimeModules { get; set; } = [];
    public List<int> DisabledModules { get; set; } = [];

    public bool? NotifyModerators { get; set; } = true;

    public bool? EnableNicknameFilter { get; set; } = true;
    public string NicknameFilter { get; set; } = " ~!@#$%^&*()_+`1234567890-=ё\"№;:?\\|qwertyuiopasdfghjklzxcvbnm{}[];'<>,./ёйцукенгшщзхъфывапролджэячсмитьбю";

    public bool PreventStackCheat { get; set; }

    public int? MaxAllowedLife { get; set; } = 500;
    public int? MaxAllowedMana { get; set; } = 200;

    public int? KillTileRange { get; set; } = 64;
    public int? PlaceTileRange { get; set; } = 32;
    public int? ReplaceTileRange { get; set; } = 32;

    public int? KillTileThreshold { get; set; } = 80;
    public int? PlaceTileThreshold { get; set; } = 20;
    public int? ReplaceTileThreshold { get; set; } = 20;

    public int? KillWallRange { get; set; } = 32;
    public int? PlaceWallRange { get; set; } = 32;
    public int? ReplaceWallRange { get; set; } = 32;

    public int? KillWallThreshold { get; set; } = 50;
    public int? PlaceWallThreshold { get; set; } = 50;
    public int? ReplaceWallThreshold { get; set; } = 50;

    public bool DisableItemDropThreshold { get; set; }
    public int? ItemDropThreshold { get; set; } = 8;
    public bool? ReturnDroppedItemInThreshold { get; set; } = true;

    public int? MaxProjectilesPerUser { get; set; } = 60;
    public int? ProjectileCreateThreshold { get; set; } = 45;

    public Dictionary<int, float> ProjectileFixedAI1 { get; set; } = new Dictionary<int, float>
    {
        { 950, 0 }
    };

    public Dictionary<int, float> ProjectileMinAI1 { get; set; } = new Dictionary<int, float>
    {
        { 611, -1 }
    };

    public Dictionary<int, float> ProjectileMaxAI1 { get; set; } = new Dictionary<int, float>
    {
        { 611, 1 }
    };

    public Dictionary<int, float> ProjectileFixedAI2 { get; set; } = [];
    public Dictionary<int, float> ProjectileMinAI2 { get; set; } = new Dictionary<int, float>
    {
        { 405, 0f },
        { 410, 0f },
        { 424, 0.5f },
        { 425, 0.5f },
        { 426, 0.5f },
        { 522, 0 },
        { 612, 0.4f },
        { 953, 0.85f },
        { 756, 0.5f },
    };

    public Dictionary<int, float> ProjectileMaxAI2 { get; set; } = new Dictionary<int, float>
    {
        { 405, 1.2f },
        { 410, 1.2f },
        { 424, 0.8f },
        { 425, 0.8f },
        { 426, 0.8f },
        { 522, 40f },
        { 612, 0.7f },
        { 953, 2 },
        { 756, 1 }
    };

    public bool DisableSwitchingPvP { get; set; }

    public bool DisableSwitchingTeam { get; set; }

    public bool DisableChestNameFilter { get; set; }
    public int? ChestFateThreshold { get; set; } = 8;

    public bool DisableHealCombatText { get; set; }
    public int? HealTextThreshold { get; set; } = 1;

    public bool DisableManaHealCombatText { get; set; }
    public int? ManaHealTextThreshold { get; set; } = 4;

    public List<string> AllowedMessages { get; set; } =
    [
        "security_tile_safety",
        "security_tile_bans"
    ];
}
