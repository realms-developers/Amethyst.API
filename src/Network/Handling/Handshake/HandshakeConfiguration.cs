using Amethyst.Storages.Config;
using Terraria;

namespace Amethyst.Network.Handling.Handshake;

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
}