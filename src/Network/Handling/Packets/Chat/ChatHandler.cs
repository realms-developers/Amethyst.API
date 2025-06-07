using Amethyst.Network.Handling.Base;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Chat;

namespace Amethyst.Network.Handling.Packets.Chat;

public sealed class ChatHandler : INetworkHandler
{
    public string Name => "net.amethyst.ChatHandler";

    public void Load()
    {
        NetworkManager.AddDirectHandler(82, OnReadNetModule);
    }

    public void Unload()
    {
        NetworkManager.RemoveDirectHandler(82, OnReadNetModule);
    }

    private static void OnReadNetModule(PlayerEntity plr, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        FastPacketReader reader = new FastPacketReader(rawPacket, 3);
        ushort type = reader.ReadUInt16();

        if (type != 1)
        {
            return;
        }

        string cmd = reader.ReadString();
        if (cmd.Length > 64)
        {
            return;
        }

        string message = reader.ReadString();
        if (message.Length > 150)
        {
            AmethystLog.System.Error("ChatHandler", $"Player {plr.Index} sent a text longer than 150 characters: {message}");
            plr.User?.Messages.ReplyError("network.chatMessageTooLong");
            return;
        }

        if (cmd == "Say" && message.StartsWith('/'))
        {
            plr.User?.Commands.RunCommand(message);
            return;
        }
        else if (cmd != "Say")
        {
            plr.User?.Commands.RunCommand($"/{cmd.ToLowerInvariant()}{(message.Length == 0 ? "" : " " + message)}");
            return;
        }

        ServerChat.HandleMessage(plr, message);
    }
}
