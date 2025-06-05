using Amethyst.Storages.Config;
using Terraria;

namespace Amethyst.Server.Entities.Players.Handshake;

public sealed class HandshakeConfiguration
{
    static HandshakeConfiguration() => Configuration.Load();

    public static Configuration<HandshakeConfiguration> Configuration { get; } = new(typeof(HandshakeConfiguration).FullName!, new());
    public static HandshakeConfiguration Instance => Configuration.Data;

    public string[] AllowedProtocols { get; set; } = ["Terraria" + Main.curRelease];
    public string[] BannedProtocols { get; set; } = ["tshock"];
    public bool AllowUnknownProtocols { get; set; }

    public bool DiscardUnwhitelistedIPs { get; set; }
    public List<string> IPWhitelist { get; set; } = [];
    public List<string> IPBlacklist { get; set; } = [];

    public bool EnableNicknameFilter { get; set; } = true;
    public string NicknameFilter { get; set; } = " ~!@#$%^&*()_+`1234567890-=ё\"№;:?\\|qwertyuiopasdfghjklzxcvbnm{}[];'<>,./ёйцукенгшщзхъфывапролджэячсмитьбю";
    public List<string> NicknameBanwords { get; set; } = [];
    public int MaxNicknameLength { get; set; } = 20;
    public int MinNicknameLength { get; set; } = 3;
    public bool AllowDuplicateNicknames { get; set; }
    public bool AllowCreativeInNormalWorld { get; set; }
    public bool AllowNormalInCreativeWorld { get; set; }

    public int MaxUsersWithSameUUID { get; set; } = 1;
    public bool DiscardInvalidGUIDs { get; set; } = true;

    public int SectionRangeLeft { get; set; } = 4;
    public int SectionRangeRight { get; set; } = 2;
    public int SectionRangeTop { get; set; } = 4;
    public int SectionRangeBottom { get; set; } = 2;
}
