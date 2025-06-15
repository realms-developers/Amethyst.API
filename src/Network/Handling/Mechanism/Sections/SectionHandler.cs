using Amethyst.Hooks;
using Amethyst.Hooks.Args.Utility;
using Amethyst.Hooks.Base;
using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Server.Entities.Players.Sections;

namespace Amethyst.Network.Handling.Mechanism.Sections;

public sealed class SectionHandler : INetworkHandler
{
    public string Name => "net.amethyst.SectionHandler";

    public void Load()
    {
        HookRegistry.GetHook<SecondTickArgs>()
            ?.Register(HandleSecondTick);
    }

    private int _tickCount;
    private void HandleSecondTick(in SecondTickArgs args, HookResult<SecondTickArgs> result)
    {
        _tickCount++;
        if (_tickCount < 5)
        {
            return;
        }

        _tickCount = 0;

        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Phase != ConnectionPhase.Connected)
            {
                continue;
            }

            PlayerSections sections = plr.Sections;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int sectionX = sections.SectionX + i;
                    int sectionY = sections.SectionY + j;

                    CheckSection(plr, sections, sectionX, sectionY);
                }
            }
        }
    }

    private void CheckSection(PlayerEntity plr, PlayerSections sections, int sectionX, int sectionY)
    {
        if (!sections.IsValidSection(sectionX, sectionY))
        {
            return;
        }

        if (sections.IsSent(sectionX, sectionY))
        {
            return;
        }

        plr.SendSection(sectionX, sectionY);
    }

    public void Unload()
    {
        HookRegistry.GetHook<SecondTickArgs>()
            ?.Unregister(HandleSecondTick);
    }
}
