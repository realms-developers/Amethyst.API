using Amethyst.Network.Handling.Packets.Handshake;
using Amethyst.Server.Entities;
using Amethyst.Server.Entities.Players;
using Amethyst.Systems.Users.Telemetry.Storage;

namespace Amethyst.Systems.Users.Telemetry;

public static class AmethystTelemetry
{
    public static UserFindResult? Find(string name)
    {
        foreach (PlayerEntity plr in EntityTrackers.Players)
        {
            if (plr.Phase == ConnectionPhase.Connected && plr.Name == name)
            {
                return new UserFindResult(plr.Name, plr.HashedUUID, plr.IP);
            }
        }

        UserInfoModel? userInfo = TelemetryStorage.Users.Find(name);
        if (userInfo != null)
        {
            return new UserFindResult(userInfo.Name, userInfo.UUIDs.Last(), userInfo.IPs.Last());
        }
        return null;
    }

    public static UserInfoModel GetOrCreateUserInfo(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("User name cannot be null or empty.", nameof(name));
        }

        UserInfoModel userInfo = TelemetryStorage.Users.Find(name) ?? new UserInfoModel(name);
        return userInfo;
    }

    public static void SaveData(PlayerEntity player)
    {
        if (player.User == null)
        {
            return;
        }

        UserInfoModel userInfo = GetOrCreateUserInfo(player.Name);
        userInfo.TryAddUUID(player.UUID);
        userInfo.TryAddIP(player.IP);
        userInfo.TryAddPlatform(player.PlatformType);
        userInfo.LastLogin = DateTime.UtcNow;

        userInfo.Save();
    }

    public static string SelfHash(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));
        }

        byte[] bytes = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
