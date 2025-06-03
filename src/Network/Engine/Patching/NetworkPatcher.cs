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
        AmethystLog.Network.Debug(nameof(NetworkPatcher), "Initializing SocketPatching...");

        // Hook the SendData method to our custom implementation
        var sendDataMethod = typeof(NetMessage).GetMethod("SendData");
        if (sendDataMethod != null)
        {
            var hook = new Hook(
                sendDataMethod,
                new Action<int, int, int, NetworkText, int, float, float, float, int, int, int>(SendDataPatched)
            );
            hook.Apply();

            AmethystLog.Network.Debug(nameof(NetworkPatcher), "SocketPatching initialized successfully.");
        }
        else
        {
            AmethystLog.Network.Debug(nameof(NetworkPatcher), "Failed to find Netplay.SendData method for hooking.");
        }
    }
    public static void SendDataPatched(int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2 , float number3, float number4, int number5, int number6, int number7)
    {
        AmethystLog.Network.Debug(nameof(NetworkPatcher), $"SendDataPatched called with msgType: {msgType}, remoteClient: {remoteClient}, ignoreClient: {ignoreClient}, text: {text}, number: {number}, number2: {number2}, number3: {number3}, number4: {number4}, number5: {number5}, number6: {number6}, number7: {number7}");
    }
}
