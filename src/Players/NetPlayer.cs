using System.Globalization;
using Amethyst.Commands;
using Amethyst.Core;
using Amethyst.Network;
using Amethyst.Network.Managing;
using Amethyst.Permissions;
using Amethyst.Players.Auth;
using Amethyst.Players.Extensions;
using Amethyst.Players.SSC;
using Amethyst.Players.SSC.Enums;
using Amethyst.Players.SSC.Interfaces;
using Amethyst.Security;
using Amethyst.Security.Limits;
using Amethyst.Text;
using Microsoft.Xna.Framework;
using Terraria;

namespace Amethyst.Players;

public sealed class NetPlayer : ICommandSender, IPermissionable, IDisposable
{
    internal static bool JailItemBanCheck(NetPlayer player)
    {
        List<int> slots = player._weirdSlots; // important memory copy
        List<int> realWeirdSlots = new List<int>(player._weirdSlots.Capacity);
        List<int> holdingBannedItems = [];

        foreach (int slot in slots)
        {
            NetItem item = player.Character[slot];

            if (SecurityManager.ItemBans.Contains(item.ID))
            {
                if (!holdingBannedItems.Contains(item.ID))
                {
                    holdingBannedItems.Add(item.ID);
                }

                realWeirdSlots.Add(slot);
            }
        }

        player._weirdSlots = realWeirdSlots;
        player._holdingBannedItems = holdingBannedItems;

        return realWeirdSlots.Count > 0;
    }

    private static readonly Color _replyErrorColor = new(201, 71, 71);
    private static readonly Color _replyInfoColor = new(191, 201, 71);
    private static readonly Color _replySuccessColor = new(71, 201, 75);
    private static readonly Color _replyWarningColor = new(201, 125, 71);

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

        Rules = new PlayerRules(this);

        Jail = new PlayerJail(this);

        _packetThreshold = new CounterThreshold(255);
        foreach (KeyValuePair<int, int> limit in SecurityManager.Configuration.PerSecondLimitPackets)
        {
            _packetThreshold.Setup(limit.Key, limit.Value);
        }

        _moduleThreshold = new CounterThreshold(255);
        foreach (KeyValuePair<int, int> limit in SecurityManager.Configuration.PerSecondLimitModules)
        {
            _moduleThreshold.Setup(limit.Key, limit.Value);
        }

        _sentPackets = new bool[255];
        _sentModules = new bool[255];

        _securityThreshold = new CounterThreshold(10);
        _securityThreshold.Setup(0, SecurityManager.Configuration.KillTileThreshold!.Value);
        _securityThreshold.Setup(1, SecurityManager.Configuration.PlaceTileThreshold!.Value);
        _securityThreshold.Setup(2, SecurityManager.Configuration.ReplaceTileThreshold!.Value);
        _securityThreshold.Setup(3, SecurityManager.Configuration.KillWallThreshold!.Value);
        _securityThreshold.Setup(4, SecurityManager.Configuration.PlaceWallThreshold!.Value);
        _securityThreshold.Setup(5, SecurityManager.Configuration.ReplaceWallThreshold!.Value);
        _securityThreshold.Setup(6, SecurityManager.Configuration.ItemDropThreshold!.Value);
        _securityThreshold.Setup(7, SecurityManager.Configuration.ProjectileCreateThreshold!.Value);
        _securityThreshold.Setup(8, SecurityManager.Configuration.ChestFateThreshold!.Value);
        _securityThreshold.Setup(9, SecurityManager.Configuration.HealTextThreshold!.Value);

        if (AuthManager.Configuration.EnableAuthorization)
        {
            Auth = new PlayerAuth(this);
        }

        _lastPos = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);

        Platform = PlayerPlatform.PC;

        Character = new ClientCharacterWrapper(this, new CharacterModel(Name));

        Jail.AddCheck(JailItemBanCheck);
    }

    public int Index { get; }

    public string Name => _playerName;

    public string IP { get; set; }

    public string UUID { get; set; }

    public SenderType Type => SenderType.RealPlayer;

    public Player TPlayer => Main.player[Index];

    public INetworkClient Socket => NetworkManager.Provider.GetClient(Index)!;

    public ICharacterWrapper Character { get; internal set; }

    public LocalPlayerUtils Utils { get; }

    public PlayerRules Rules { get; }

    public PlayerJail Jail { get; }

    public PlayerAuth? Auth { get; }

    /// <summary>
    /// Indicates that player is connected to server
    /// </summary>
    public bool IsActive => Socket.IsConnected && !Socket.IsFrozen;

    /// <summary>
    /// Indicates that player is connected to server
    /// </summary>
    public bool IsJoined => UUID != "" && Name != "" && _wasSpawned;

    public bool IsHeldItemBanned => Utils.HeldItem.type >= 0 && Utils.HeldItem.stack > 0 && SecurityManager.ItemBans.Contains(Utils.HeldItem.type);

    /// <summary>
    /// Indicates that player is capable (can run commands, modify world and etc...)
    /// </summary>
    public bool IsCapable => IsActive && IsJoined && (Auth == null || Auth.IsAuthorized);

    public bool IsRootGranted { get; set; }

    public string Language { get; set; }

    public PlayerPlatform Platform { get; set; }

    public IReadOnlyList<int> HoldingBannedItems => _holdingBannedItems.AsReadOnly();

    internal List<int> _holdingBannedItems = [];

    internal CounterThreshold _securityThreshold;

    private string _playerName;
    internal CounterThreshold _packetThreshold;
    internal CounterThreshold _moduleThreshold;

    internal bool[] _sentPackets;
    internal bool[] _sentModules;

    private readonly Dictionary<Type, IPlayerExtension> _extensions;

    internal bool _wasSpawned;
    internal bool _sentSpawnPacket; // used for preventing anonymous clients
    internal bool _sentPlatformPacket;

    internal short _lastLife;
    internal short _lastMaxLife;

    internal short _lastMana;
    internal short _lastMaxMana;

    internal Vector2 _lastPos;

    internal PlayerInfo1 _initInfo1;
    internal PlayerInfo2 _initInfo2;
    internal PlayerInfo3 _initInfo3;
    internal byte _initSkinVariant;
    internal byte _initHair;
    internal byte _initHairDye;
    internal bool[] _initHideAccessories = new bool[10];
    internal byte _initHideMisc;
    internal NetColor[] _initColors = new NetColor[7];
    internal Dictionary<string, DateTime> _notifyDelay = new Dictionary<string, DateTime>();

    internal List<int> _weirdSlots = [];

    public bool CanNotify(string messageType, TimeSpan delay)
    {
        if (!_notifyDelay.TryGetValue(messageType, out DateTime value))
        {
            value = DateTime.UtcNow.Add(delay);
            _notifyDelay.Add(messageType, value);
            return true;
        }

        if (value < DateTime.UtcNow)
        {
            _notifyDelay[messageType] = DateTime.UtcNow.Add(delay);
            return true;
        }

        return false;
    }

    internal void UnloadExtensions()
    {
        foreach (KeyValuePair<Type, IPlayerExtension> kvp in _extensions)
        {
            kvp.Value.Unload();
        }

        _extensions.Clear();
    }

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

        if (_extensions.TryGetValue(type, out IPlayerExtension? ext))
        {
            ext?.Unload();
        }

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

    public void SendMessage(string text) => SendMessage(text, Color.White);

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

    public void ReplyMessage(string text) => ReplyMessage(text, Color.White);

    public void ReplyMessage(string text, Color color)
        => SendMessage($"[c/303030:{Localization.Get("amethyst.serverPrefix", Language)}:] {text}", color);

    public void ReplyError(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replyErrorColor);

    public void ReplyInfo(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replyInfoColor);

    public void ReplySuccess(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replySuccessColor);

    public void ReplyWarning(string text, params object[] args)
        => ReplyMessage(string.Format(CultureInfo.InvariantCulture, Localization.Get(text, Language), args), _replyWarningColor);

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
        AmethystLog.Network.Error(nameof(NetPlayer), $"Player '{Name}' was kicked for reason: {reason}");

        Socket.Disconnect(string.Format(CultureInfo.InvariantCulture, Localization.Get(reason, Language), args ?? []));
    }

    public void Dispose()
    {
        _packetThreshold.Dispose();
        _moduleThreshold.Dispose();

        GC.SuppressFinalize(this);
    }
}
