using Amethyst.Core;
using Amethyst.Network.Packets;
using Amethyst.Text;

namespace Amethyst.Network.Managing;

public sealed class PacketHandleResult
{
    internal PacketHandleResult(IPacket packet)
    {
        _packet = packet;
        _reasons = new List<string>(16);
    }

    private bool _handled;
    private List<string> _reasons;
    private IPacket _packet;

    public bool IsHandled => _handled;
    public IReadOnlyList<string> Reasons => _reasons.AsReadOnly();

    public void Ignore(string reason)
    {
        _handled = true;
        _reasons.Add(reason);
    }

    internal void Log()
    {
        if (_reasons.Count == 0 || AmethystSession.Profile.DebugMode == false) return;

        var lines = PagesCollection.PageifyItems(_reasons, 150);

        AmethystLog.Network.Debug("PacketHandleResult", $"Ignored packet {_packet.PacketID}, reasons:");
        lines.ForEach(p => AmethystLog.Network.Debug("PacketHandleResult", p));
    }
}