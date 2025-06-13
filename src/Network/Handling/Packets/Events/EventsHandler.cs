using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Server.Entities.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.Enums;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;

namespace Amethyst.Network.Handling.Packets.Events;

public sealed class EventsHandler : INetworkHandler
{
    public string Name => "net.amethyst.EventsHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<EventBirthdayParty>(OnEventBirthdayParty);
        NetworkManager.SetMainHandler<EventDD2AttemptSkipWaitTime>(OnEventDD2AttemptSkipWaitTime);
        NetworkManager.SetMainHandler<EventDD2Toggle>(OnEventDD2Toggle);
        NetworkManager.SetMainHandler<EventInvokeV1>(OnEventInvokeV1);
        NetworkManager.SetMainHandler<EventInvokeV2>(OnEventInvokeV2);
        NetworkManager.SetMainHandler<EventCreditsOrTransform>(OnEventCreditsOrTransform);
    }

    private void OnEventCreditsOrTransform(PlayerEntity plr, ref EventCreditsOrTransform packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.EventType == 1)
        {
            NPC.TransformCopperSlime(packet.ExtraValue);
        }
        else if (packet.EventType == 2)
        {
            NPC.TransformElderSlime(packet.ExtraValue);
        }
    }

    private void OnEventInvokeV2(PlayerEntity plr, ref EventInvokeV2 packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (packet.EventOrBossType >= 0 && packet.EventOrBossType < NPCID.Count && NPCID.Sets.MPAllowedEnemies[packet.EventOrBossType])
        {
            if (!NPC.AnyNPCs(packet.EventOrBossType))
            {
                NPC.SpawnOnPlayer(plr.Index, packet.EventOrBossType);
            }
        }
        else if (packet.EventOrBossType == -4)
        {
            if (!Main.dayTime && !DD2Event.Ongoing)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[31].Key), new Color(50, 255, 130));
                Main.startPumpkinMoon();
                NetMessage.TrySendData(7);
                NetMessage.TrySendData(78, -1, -1, NetworkText.Empty, 0, 1f, 2f, 1f);
            }
        }
        else if (packet.EventOrBossType == -5)
        {
            if (!Main.dayTime && !DD2Event.Ongoing)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[34].Key), new Color(50, 255, 130));
                Main.startSnowMoon();
                NetMessage.TrySendData(7);
                NetMessage.TrySendData(78, -1, -1, NetworkText.Empty, 0, 1f, 1f, 1f);
            }
        }
        else if (packet.EventOrBossType == -6)
        {
            if (Main.dayTime && !Main.eclipse)
            {
                if (Main.remixWorld)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[106].Key), new Color(50, 255, 130));
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[20].Key), new Color(50, 255, 130));
                }
                Main.eclipse = true;
                NetMessage.TrySendData(7);
            }
        }
        else if (packet.EventOrBossType == -7)
        {
            Main.invasionDelay = 0;
            Main.StartInvasion(4);
            NetMessage.TrySendData(7);
            NetMessage.TrySendData(78, -1, -1, NetworkText.Empty, 0, 1f, Main.invasionType + 3);
        }
        else if (packet.EventOrBossType == -8)
        {
            if (NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
            {
                WorldGen.StartImpendingDoom(720);
                NetMessage.TrySendData(7);
            }
        }
        else if (packet.EventOrBossType == -10)
        {
            if (!Main.dayTime && !Main.bloodMoon)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey(Lang.misc[8].Key), new Color(50, 255, 130));
                Main.bloodMoon = true;
                if (Main.GetMoonPhase() == MoonPhase.Empty)
                {
                    Main.moonPhase = 5;
                }
                AchievementsHelper.NotifyProgressionEvent(4);
                NetMessage.TrySendData(7);
            }
        }
        else if (packet.EventOrBossType == -11)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.CombatBookUsed"), new Color(50, 255, 130));
            NPC.combatBookWasUsed = true;
            NetMessage.TrySendData(7);
        }
        else if (packet.EventOrBossType == -12)
        {
            NPC.UnlockOrExchangePet(ref NPC.boughtCat, 637, "Misc.LicenseCatUsed", packet.EventOrBossType);
        }
        else if (packet.EventOrBossType == -13)
        {
            NPC.UnlockOrExchangePet(ref NPC.boughtDog, 638, "Misc.LicenseDogUsed", packet.EventOrBossType);
        }
        else if (packet.EventOrBossType == -14)
        {
            NPC.UnlockOrExchangePet(ref NPC.boughtBunny, 656, "Misc.LicenseBunnyUsed", packet.EventOrBossType);
        }
        else if (packet.EventOrBossType == -15)
        {
            NPC.UnlockOrExchangePet(ref NPC.unlockedSlimeBlueSpawn, 670, "Misc.LicenseSlimeUsed", packet.EventOrBossType);
        }
        else if (packet.EventOrBossType == -16)
        {
            NPC.SpawnMechQueen(plr.Index);
        }
        else if (packet.EventOrBossType == -17)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.CombatBookVolumeTwoUsed"), new Color(50, 255, 130));
            NPC.combatBookVolumeTwoWasUsed = true;
            NetMessage.TrySendData(7);
        }
        else if (packet.EventOrBossType == -18)
        {
            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Misc.PeddlersSatchelUsed"), new Color(50, 255, 130));
            NPC.peddlersSatchelWasUsed = true;
            NetMessage.TrySendData(7);
        }
        else if (packet.EventOrBossType < 0)
        {
            int num217 = 1;
            if (packet.EventOrBossType > -InvasionID.Count)
            {
                num217 = -packet.EventOrBossType;
            }
            if (num217 > 0 && Main.invasionType == 0)
            {
                Main.invasionDelay = 0;
                Main.StartInvasion(num217);
            }
            NetMessage.TrySendData(78, -1, -1, NetworkText.Empty, 0, 1f, Main.invasionType + 3);
        }
    }

    private void OnEventInvokeV1(PlayerEntity plr, ref EventInvokeV1 packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        switch (packet.EventType)
        {
            case 1:
                NPC.SpawnSkeletron(plr.Index);
                break;
            case 2:
                NetMessage.TrySendData(51, -1, plr.Index, NetworkText.Empty, plr.Index, (int)packet.EventType);
                break;
            case 3:
                if (Main.netMode == 2)
                {
                    Main.Sundialing();
                }
                break;
            case 4:
                Main.npc[packet.EventAdditionalValue].BigMimicSpawnSmoke();
                break;
            case 5:
                NPC nPC6 = new();
                nPC6.SetDefaults(664);
                Main.BestiaryTracker.Kills.RegisterKill(nPC6);
                break;
            case 6:
                if (Main.netMode == 2)
                {
                    Main.Moondialing();
                }
                break;
        }
    }

    private void OnEventDD2Toggle(PlayerEntity plr, ref EventDD2Toggle packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        if (DD2Event.WouldFailSpawningHere(packet.X, packet.Y))
        {
            DD2Event.FailureMessage(plr.Index);
        }
        DD2Event.SummonCrystal(packet.X, packet.Y, plr.Index);
    }

    private void OnEventDD2AttemptSkipWaitTime(PlayerEntity plr, ref EventDD2AttemptSkipWaitTime packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        DD2Event.AttemptToSkipWaitTime();
    }

    private void OnEventBirthdayParty(PlayerEntity plr, ref EventBirthdayParty packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected)
        {
            return;
        }

        BirthdayParty.ToggleManualParty();
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<EventBirthdayParty>(null);
        NetworkManager.SetMainHandler<EventDD2AttemptSkipWaitTime>(null);
        NetworkManager.SetMainHandler<EventDD2Toggle>(null);
        NetworkManager.SetMainHandler<EventInvokeV1>(null);
        NetworkManager.SetMainHandler<EventInvokeV2>(null);
        NetworkManager.SetMainHandler<EventCreditsOrTransform>(null);
    }
}
