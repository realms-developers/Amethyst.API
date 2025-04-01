using System.Globalization;
using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Permissions;
using Amethyst.Players.Extensions;
using Amethyst.Players.SSC.Interfaces;
using Amethyst.Text;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Players;

public sealed class NetPlayer : ICommandSender, IPermissionable
{
    internal NetPlayer(int index)
    {
        Index = index;

        IP = "0.0.0.0";
        UUID = "";

        _playerName = "";
        _extensions = [];

        Language = AmethystSession.Profile.DefaultLanguage;

        PlayerExtensions.LoadExtensions(this);

        Utils = new LocalPlayerUtils(this);
    }

    private string _playerName;

    public int Index { get; }

    public string Name => _playerName;

    public string IP { get; set; }

    public string UUID { get; set; }

    public SenderType Type => SenderType.RealPlayer;

    public Player TPlayer => Main.player[Index];

    public INetworkClient Socket => NetworkManager.Provider.GetClient(Index)!;

    public ICharacterWrapper? Character { get; internal set; }

    public LocalPlayerUtils Utils { get; }

    /// <summary>
    /// Indicates that player is connected to server
    /// </summary>
    public bool IsActive => Socket.IsConnected && !Socket.IsFrozen;

    /// <summary>
    /// Indicates that player is capable (can run commands, modify world and etc...)
    /// </summary>
    public bool IsCapable => IsActive && UUID != "" && Name != "" && _wasSpawned;

    public bool IsRootGranted { get; set; }

    public string Language { get; set; }

    private readonly Dictionary<Type, IPlayerExtension> _extensions;

    internal bool _wasSpawned;
    internal bool _sentSpawnPacket; // used for preventing anonymous clients

    public void LoadExtension<T>() where T : IPlayerExtension
    {
        Type type = typeof(T);

        if (_extensions.ContainsKey(type))
        {
            return;
        }

        IPlayerExtensionBuilder<T>? builder =
            PlayerExtensions.GetBuilder<T>() ?? throw new InvalidOperationException($"Cannot find IPlayerExtensionBuilder<{type.Name}> in ExtensionsManager.");

        T ext = builder.Build(this);
        ext.Load();

        _extensions.Add(type, ext);
    }

    public void UnloadExtension<T>() where T : IPlayerExtension
    {
        Type type = typeof(T);
        _extensions.Remove(type);
    }

    public T? GetExtension<T>() where T : IPlayerExtension
    {
        Type type = typeof(T);

        if (_extensions.TryGetValue(type, out IPlayerExtension? ext))
        {
            return (T)ext;
        }

        return default;
    }

    public bool HasPermission(string permission)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasPermission(this, permission)) == PermissionAccess.HasPermission);

    public bool HasChestPermission(int x, int y)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasChestPermission(this, x, y)) == PermissionAccess.HasPermission);

    public bool HasChestEditPermission(int x, int y)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasChestEditPermission(this, x, y)) == PermissionAccess.HasPermission);

    public bool HasSignPermission(int x, int y)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasSignPermission(this, x, y)) == PermissionAccess.HasPermission);

    public bool HasSignEditPermission(int x, int y)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasSignEditPermission(this, x, y)) == PermissionAccess.HasPermission);

    public bool HasTEPermission(int x, int y)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasTEPermission(this, x, y)) == PermissionAccess.HasPermission);

    public bool HasTilePermission(int x, int y, int? width = null, int? height = null)
        => IsCapable && (IsRootGranted || AmethystSession.PlayerPermissions.HandleResult(p => p.HasTilePermission(this, x, y, width, height)) == PermissionAccess.HasPermission);

    public void SendMessage(string text, Color color)
    {
        byte[] packet = new PacketWriter().SetType(82)
            .PackUInt16(1) // text id
            .PackByte(255)
            .PackByte(0)
            .PackString(text)
            .PackColor(color)
            .BuildPacket();

        Socket.SendPacket(packet);
    }


    public void ReplyMessage(string text, Color color)
        => SendMessage($"[c/303030:{Localization.Get("amethyst.serverPrefix", Language)}:] {text}", color);

    public void ReplyError(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), Color.Red);

    public void ReplyInfo(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), Color.Yellow);

    public void ReplySuccess(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), Color.Green);

    public void ReplyWarning(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), Color.Orange);

    public void ReplyPage(PagesCollection pages, string? header, string? footer, object[]? footerArgs, bool showPageName, int page = 0)
        => pages.SendPage(this, header, footer, footerArgs, showPageName, page);

    public void SetName(string name, bool networkUpdate = true)
    {
        TPlayer.name = name;
        _playerName = name;

        if (networkUpdate)
        {
            NetMessage.TrySendData(4);
        }
    }

    public void Kick(string reason, object[]? args = null)
    {
        AmethystLog.Network.Error("Players", $"Player '{Name}' was kicked for reason: {reason}");

        Socket.Disconnect(string.Format(CultureInfo.InvariantCulture, Localization.Get(reason, Language), args ?? []));
    }
}
