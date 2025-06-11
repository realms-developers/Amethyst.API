using Amethyst.Hooks;
using Amethyst.Hooks.Args.Players;
using Amethyst.Hooks.Context;
using Amethyst.Kernel;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Characters;

namespace Amethyst.Network.Handling.Packets.Characters;

public sealed class CharactersHandler : INetworkHandler
{
    public string Name => "net.amethyst.CharactersHandler";

    public void Load()
    {
        NetworkManager.AddHandler<PlayerInfo>(OnPlayerInfo);
        NetworkManager.AddHandler<PlayerSlot>(OnPlayerSlot);
        NetworkManager.AddHandler<PlayerLife>(OnPlayerLife);
        NetworkManager.AddHandler<PlayerMana>(OnPlayerMana);
        NetworkManager.AddHandler<PlayerTownNPCQuestsStats>(OnPlayerQuests);
        NetworkManager.AddHandler<PlayerLoadout>(OnPlayerLoadout);

        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Register(OnPlayerFullyJoined);
        HookRegistry.GetHook<PlayerSetUserArgs>()
            ?.Register(OnPlayerAuthorized);
    }

    public void Unload()
    {
        NetworkManager.RemoveHandler<PlayerInfo>(OnPlayerInfo);
        NetworkManager.RemoveHandler<PlayerSlot>(OnPlayerSlot);
        NetworkManager.RemoveHandler<PlayerLife>(OnPlayerLife);
        NetworkManager.RemoveHandler<PlayerMana>(OnPlayerMana);
        NetworkManager.RemoveHandler<PlayerTownNPCQuestsStats>(OnPlayerQuests);
        NetworkManager.RemoveHandler<PlayerLoadout>(OnPlayerLoadout);

        HookRegistry.GetHook<PlayerFullyJoinedArgs>()
            ?.Unregister(OnPlayerFullyJoined);
        HookRegistry.GetHook<PlayerSetUserArgs>()
            ?.Unregister(OnPlayerAuthorized);
    }

    private void OnPlayerAuthorized(in PlayerSetUserArgs args, HookResult<PlayerSetUserArgs> result)
    {
        PlayerEntity plr = args.Player;

        if (args.New == null || args.New.Character != null)
        {
            AmethystLog.System.Verbose("CharactersHandler", $"Player {plr.Index} ({plr.Name}) already has a character assigned, skipping.");
            return;
        }

        if (AmethystSession.Profile.SSCMode)
        {
            args.New!.Character = CharactersOrganizer.ServersideFactory.BuildFor(args.New);

            args.New.Character.Handler.InReadonlyMode = true;
            AmethystLog.System.Info("CharactersHandler", $"Player {plr.Index} is in SSC mode, character handler is set to readonly mode.");
            AmethystLog.System.Info("CharactersHandler", $"Set SS-Character for player {plr.Index} ({plr.Name}).");
        }
        else
        {
            args.New!.Character = CharactersOrganizer.ClientsideFactory.BuildFor(args.New);
            AmethystLog.System.Info("CharactersHandler", $"Set CS-Character for player {plr.Index} ({plr.Name}).");
        }
    }

    private void OnPlayerLoadout(PlayerEntity plr, ref PlayerLoadout packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (packet.LoadoutIndex > 2 || packet.LoadoutIndex < 0)
        {
            ignore = true;
            AmethystLog.System.Error("CharactersHandler", $"Player {plr.Index} ({plr.Name}) tried to set an invalid loadout index: {packet.LoadoutIndex}");
            return;
        }

        if (plr.User == null || plr.User.Character == null)
        {
            plr.TempLoadoutIndex = packet.LoadoutIndex;
            return;
        }

        plr.User.Character.LoadoutIndex = packet.LoadoutIndex;
    }

    private void OnPlayerFullyJoined(in PlayerFullyJoinedArgs args, HookResult<PlayerFullyJoinedArgs> result)
    {
        PlayerEntity plr = args.Player;

        if (plr.User == null || plr.User.Character == null)
        {
            return;
        }

        plr.User.Character.LoadoutIndex = plr.TempLoadoutIndex;
        plr.User.Character.LoadModel(plr.User.Character.CurrentModel);
        plr.User.Character.Handler.InReadonlyMode = false;
    }

    private void OnPlayerQuests(PlayerEntity plr, ref PlayerTownNPCQuestsStats packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
        {
            return;
        }

        plr.User?.Character?.Handler.HandleQuests(packet);
    }

    private void OnPlayerMana(PlayerEntity plr, ref PlayerMana packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
        {
            return;
        }

        plr.User?.Character?.Handler.HandleSetMana(packet);
    }

    private void OnPlayerLife(PlayerEntity plr, ref PlayerLife packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
        {
            return;
        }

        plr.User?.Character?.Handler.HandleSetLife(packet);
    }

    private void OnPlayerSlot(PlayerEntity plr, ref PlayerSlot packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
        {
            return;
        }

        plr.User?.Character?.Handler.HandleSlot(packet);
    }

    private void OnPlayerInfo(PlayerEntity plr, ref PlayerInfo packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != Handshake.ConnectionPhase.Connected)
        {
            return;
        }

        plr.User?.Character?.Handler.HandlePlayerInfo(packet);
    }
}
