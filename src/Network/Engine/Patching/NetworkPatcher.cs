using System.Runtime.Serialization;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using MonoMod.Utils;
using Terraria;
using Terraria.Localization;

namespace Amethyst.Network.Engine.Patching;

internal static class NetworkPatcher
{
    internal static unsafe void Initialize()
    {
        On.Terraria.NetMessage.SendData += SendDataPatched;
        AmethystLog.Network.Debug(nameof(NetworkPatcher), "Patched => Terraria.NetMessage.SendData.");
    }

    private static void SendDataPatched(On.Terraria.NetMessage.orig_SendData orig, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
    {
        //AmethystLog.Network.Debug(nameof(NetworkPatcher), $"SendDataPatched called with msgType: {msgType}, remoteClient: {remoteClient}, ignoreClient: {ignoreClient}, text: {text}, number: {number}, number2: {number2}, number3: {number3}, number4: {number4}, number5: {number5}, number6: {number6}, number7: {number7}");
    }
}
