using Amethyst.Players;

namespace Amethyst.Network.Packets;

public sealed class IncomingModule : IDisposable, IPacket
{
    public IncomingModule(byte packetId, byte[] buffer, byte sender, int start, int length)
    {
        PacketID = packetId;
        Buffer = buffer;
        Sender = sender;
        Start = start;
        Length = length;
    }
    
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

        Buffer = Array.Empty<byte>();

        GC.SuppressFinalize(this);
    }

    private MemoryStream? _stream;
    private BinaryReader? _reader;

    public byte PacketID { get; }
    public NetPlayer Player => PlayerManager.Tracker[Sender];
    
    public byte[] Buffer;
    public byte Sender;
    public int Start;
    public int Length;
}