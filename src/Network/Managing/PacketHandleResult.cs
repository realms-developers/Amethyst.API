using Amethyst.Core;
using Amethyst.Network.Packets;
using Amethyst.Text;

namespace Amethyst.Network.Managing;

public sealed class PacketHandleResult
{
    public PacketHandleResult(IPacket packet)
    {
        _packet = packet;
        _reasons = new List<string>(16);
    }

    private bool _handled;
    private readonly List<string> _reasons;
    private readonly IPacket _packet;

    public bool IsHandled => _handled;
    public IReadOnlyList<string> Reasons => _reasons.AsReadOnly();

    public void Ignore(string reason)
    {
        _handled = true;
        _reasons.Add(reason);
    }

    public void Log()
    {
        if (_reasons.Count == 0 || !AmethystSession.Profile.DebugMode)
        {
            return;
        }

        List<string> lines = PagesCollection.PageifyItems(_reasons, 150);

        AmethystLog.Network.Debug(nameof(PacketHandleResult), $"Ignored packet {_packet.PacketID}, reasons:");
        lines.ForEach(p => AmethystLog.Network.Debug(nameof(PacketHandleResult), p));
    }
}
