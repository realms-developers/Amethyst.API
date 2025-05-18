using Amethyst.Infrastructure;
using Amethyst.Network.Packets;
using Amethyst.Text;

namespace Amethyst.Network.Managing;

public sealed class PacketHandleResult(IPacket packet)
{
    private bool _handled;
    private readonly List<string> _reasons = new(16);
    private readonly IPacket _packet = packet;

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
