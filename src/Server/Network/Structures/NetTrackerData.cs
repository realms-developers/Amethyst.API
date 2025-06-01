#pragma warning disable CA1051

namespace Amethyst.Server.Network.Structures;

public struct NetTrackerData
{
    public NetTrackerData()
    {
        ExpectedOwner = -1;
        ExpectedIdentity = -1;
        ExpectedType = -1;
    }

    public NetTrackerData(short expectedOwner, short expectedIdentity, short expectedType)
    {
        ExpectedOwner = expectedOwner;
        ExpectedIdentity = expectedIdentity;
        ExpectedType = expectedType;
    }

    public short ExpectedOwner;
    public short ExpectedIdentity;
    public short ExpectedType;
}
