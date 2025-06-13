using Amethyst.Network.Handling.Base;
using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Network.Packets;
using Amethyst.Network.Structures;
using Amethyst.Network.Utilities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users.Base.Permissions;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Net;

namespace Amethyst.Network.Handling.Packets.NetModules;

public sealed class NetModulesHandler : INetworkHandler
{
    public string Name => "net.amethyst.NetModulesHandler";

    public void Load()
    {
        NetworkManager.SetMainHandler<ReadNetModule>(OnReadNetModule);
    }

    private unsafe void OnReadNetModule(PlayerEntity plr, ref ReadNetModule packet, ReadOnlySpan<byte> rawPacket, ref bool ignore)
    {
        if (plr.Phase != ConnectionPhase.Connected || plr.User == null)
        {
            return;
        }

        var reader = new FastPacketReader(rawPacket, 3 + sizeof(ushort));

        switch (packet.NetModuleType)
        {
            case 0: // liquids
                if (plr.User.Permissions.HasPermission("world.liquids") != PermissionAccess.HasPermission)
                    return;

                int num = reader.ReadUInt16();
                for (int i = 0; i < num; i++)
                {
                    int num2 = reader.ReadInt32();
                    byte liquid = reader.ReadByte();
                    byte liquidType = reader.ReadByte();
                    int num3 = (num2 >> 16) & 0xFFFF;
                    int num4 = num2 & 0xFFFF;
                    Tile tile = Main.tile[num3, num4];
                    if (tile != null)
                    {
                        tile.liquid = liquid;
                        tile.liquidType(liquidType);
                    }
                }
                break;

            case 2: // map ping
                NetVector2 position = reader.ReadNetVector2();
                FastPacketWriter writer = new FastPacketWriter(82, new byte[3 + sizeof(NetVector2) + sizeof(ushort)]);
                writer.WriteUInt16(2);
                writer.WriteNetVector2(position);

                PlayerUtils.BroadcastPacketBytes(writer.BuildNoResize(), plr.Index);
                break;

            case 8: // pylon teleport
                if (reader.ReadByte() == 2)
                {
                    TeleportPylonInfo info = default;
                    info.PositionInTiles = new(reader.ReadInt16(), reader.ReadInt16());
                    info.TypeOfPylon = (TeleportPylonType)reader.ReadByte();
                    Main.PylonSystem.HandleTeleportRequest(info, plr.Index);
                }
                break;

            case 9: // particles

                ParticleOrchestraType particleOrchestraType = (ParticleOrchestraType)reader.ReadByte();
                ParticleOrchestraSettings settings = default(ParticleOrchestraSettings);
                StrippedMemoryStream stream = reader.StreamOpen();
                BinaryReader binaryReader = new(stream);
                settings.DeserializeFrom(binaryReader);
                binaryReader.Dispose();
                reader.StreamClose(stream);

                FastPacketWriter writer2 = new FastPacketWriter(82, reader.Length);
                writer2.WriteByteSpan(rawPacket.Slice(3, reader.Length - 3));
                byte[] data = writer2.BuildNoResize();

                PlayerUtils.BroadcastPacketBytes(data, plr.Index);
                break;
        }
    }

    public void Unload()
    {
        NetworkManager.SetMainHandler<ReadNetModule>(null);
    }
}
