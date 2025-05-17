using Amethyst.Players;

namespace Amethyst.Network.Packets;

public sealed class IncomingModule(byte packetId, byte[] buffer, byte sender, int start, int length) : IDisposable, IPacket
{
    public BinaryReader GetReader()
    {
        if (_reader == null || _stream == null)
        {
            _stream = new MemoryStream(Buffer);
            _reader = new BinaryReader(_stream);
        }

        _reader.BaseStream.Position = Start;
        return _reader;
    }

    public void Dispose()
    {
        _reader?.Dispose();
        _stream?.Dispose();

        Buffer = [];

        GC.SuppressFinalize(this);
    }

    private MemoryStream? _stream;
    private BinaryReader? _reader;

    public byte PacketID { get; } = packetId;
    public NetPlayer Player => PlayerManager.Tracker[Sender];

    public byte[] Buffer { get; set; } = buffer;
    public byte Sender { get; set; } = sender;
    public int Start { get; set; } = start;
    public int Length { get; set; } = length;
}
