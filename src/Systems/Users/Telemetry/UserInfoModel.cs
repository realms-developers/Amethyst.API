using Amethyst.Network.Enums;
using Amethyst.Storages.Mongo;
using Amethyst.Systems.Users.Telemetry.Storage;
using MongoDB.Bson.Serialization.Attributes;

namespace Amethyst.Systems.Users.Telemetry;

[BsonIgnoreExtraElements]
public sealed class UserInfoModel : DataModel
{
    public UserInfoModel(string name) : base(name)
    {
    }

    public List<string> UUIDs { get; set; } = new();
    public List<string> IPs { get; set; } = new();
    public List<PlatformType> Platforms { get; set; } = new();
    public List<UserSessionInfo> Sessions { get; set; } = new();
    public DateTime LastLogin { get; set; }

    public bool TryAddUUID(string uuid)
    {
        if (UUIDs.LastOrDefault() == uuid)
            return false;

        UUIDs.Add(uuid);
        return true;
    }

    public bool TryAddIP(string ip)
    {
        if (IPs.LastOrDefault() == ip)
            return false;

        IPs.Add(ip);
        return true;
    }

    public bool TryAddPlatform(PlatformType platform)
    {
        if (Platforms.LastOrDefault() == platform)
            return false;

        Platforms.Add(platform);
        return true;
    }

    public override void Save()
    {
        TelemetryStorage.Users.Save(this);
    }

    public override void Remove()
    {
        TelemetryStorage.Users.Remove(Name);
    }
}